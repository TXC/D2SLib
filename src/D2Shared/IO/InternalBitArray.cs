// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Adapted from here, to allow for span-based constructor. Removed unused/unsafe code.
// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Collections/src/System/Collections/BitArray.cs

using CommunityToolkit.HighPerformance;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace D2Shared.IO
{
    // A vector of bits.  Use this to store bits efficiently
    [Serializable]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    [DebuggerTypeProxy(typeof(BitArrayDebugView))]
    internal sealed class InternalBitArray : ICollection, IList<bool>, ICloneable, IDisposable
    {
        private int[] m_array; // Do not rename (binary serialization)
        private int m_length; // Do not rename (binary serialization)
        private int _version; // Do not rename (binary serialization)

        private const int _ShrinkThreshold = 256;

        /// <summary>
        /// Allocates space to hold length bit values. All of the values in the bit
        /// array are set to false.
        /// </summary>
        /// <param name="length"></param>
        /// <exception cref="ArgumentException">if length < 0</exception>
        public InternalBitArray(int length)
            : this(length, false)
        {
        }

        /// <summary>
        /// Allocates space to hold length bit values. All of the values in the bit
        /// array are set to defaultValue.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="defaultValue"></param>
        /// <exception cref="ArgumentOutOfRangeException">if length < 0</exception>
        public InternalBitArray(int length, bool defaultValue)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, "Length must be non-negative");
            }

            int arrayLength = GetInt32ArrayLengthFromBitLength(length);
            m_array = ArrayPool<int>.Shared.Rent(arrayLength);
            m_length = length;

            if (defaultValue)
            {
                var span = m_array.AsSpan(0, arrayLength);
                span.Fill(-1);

                // clear high bit values in the last int
                Div32Rem(length, out int extraBits);
                if (extraBits > 0)
                {
                    span[^1] = (1 << extraBits) - 1;
                }
            }

            _version = 0;
        }

        /// <summary>
        /// Allocates space to hold the bit values in bytes. bytes[0] represents
        /// bits 0 - 7, bytes[1] represents bits 8 - 15, etc. The LSB of each byte
        /// represents the lowest index value; bytes[0] & 1 represents bit 0,
        /// bytes[0] & 2 represents bit 1, bytes[0] & 4 represents bit 2, etc.
        /// </summary>
        /// <param name="bytes"></param>
        /// <exception cref="ArgumentException">if bytes == null</exception>
        public InternalBitArray(ReadOnlySpan<byte> bytes)
        {
            // this value is chosen to prevent overflow when computing m_length.
            // m_length is of type int32 and is exposed as a property, so
            // type of m_length can't be changed to accommodate.
            if (bytes.Length > int.MaxValue / BitsPerByte)
            {
                throw new ArgumentException("Too many bytes!", nameof(bytes));
            }

            m_array = ArrayPool<int>.Shared.Rent(GetInt32ArrayLengthFromByteLength(bytes.Length));
            m_length = bytes.Length * BitsPerByte;

            uint totalCount = (uint)bytes.Length / 4;

            for (int i = 0; i < totalCount; i++)
            {
                m_array[i] = BinaryPrimitives.ReadInt32LittleEndian(bytes);
                bytes = bytes[4..];
            }

            Debug.Assert(bytes.Length is >= 0 and < 4);

            int last = 0;
            switch (bytes.Length)
            {
                case 3:
                    last = bytes[2] << 16;
                    goto case 2;
                // fall through
                case 2:
                    last |= bytes[1] << 8;
                    goto case 1;
                // fall through
                case 1:
                    m_array[totalCount] = last | bytes[0];
                    break;
            }

            _version = 0;
        }

        /// <summary>
        /// Allocates a new InternalBitArray with the same length and bit values
        /// as bits.
        /// </summary>
        /// <param name="bits"></param>
        /// <exception cref="ArgumentNullException">if bits == null</exception>
        public InternalBitArray(InternalBitArray bits)
        {
            if (bits == null)
            {
                throw new ArgumentNullException(nameof(bits));
            }

            int arrayLength = GetInt32ArrayLengthFromBitLength(bits.m_length);

            m_array = ArrayPool<int>.Shared.Rent(arrayLength);

            Debug.Assert(bits.m_array.Length <= arrayLength);

            Array.Copy(bits.m_array, m_array, arrayLength);
            m_length = bits.m_length;

            _version = bits._version;
        }

        /// <summary>
        /// Allocates a new BitArray with the same length and bit values as bits.
        /// </summary>
        /// <param name="bits"></param>
        /// <exception cref="ArgumentNullException">if bits == null</exception>
        public InternalBitArray(BitArray bits)
        {
            if (bits == null)
            {
                throw new ArgumentNullException(nameof(bits));
            }

            int arrayLength = GetInt32ArrayLengthFromBitLength(bits.Length);

            m_array = ArrayPool<int>.Shared.Rent(arrayLength);

            Debug.Assert(bits.Length <= arrayLength);

            for (int i = 0; i < bits.Length; i++)
            {
                Set(i, bits[i]);
            }
            _version = 0;
        }

        public bool this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Get(index);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Set(index, value);
        }

        /// <summary>
        /// Returns the bit value at position index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">if index < 0 or index >= GetLength()</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Get(int index)
        {
            if ((uint)index >= (uint)m_length)
            {
                ThrowArgumentOutOfRangeException(index);
            }

            return (m_array[index >> 5] & (1 << index)) != 0;
        }

        /// <summary>
        /// Sets the bit value at position index to value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException">if index < 0 or index >= GetLength()</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Set(int index, bool value)
        {
            if ((uint)index >= (uint)m_length)
            {
                ThrowArgumentOutOfRangeException(index);
            }

            int bitMask = 1 << index;
            ref int segment = ref m_array[index >> 5];

            if (value)
            {
                segment |= bitMask;
            }
            else
            {
                segment &= ~bitMask;
            }

            _version++;
        }

        public void Add(bool value)
        {
            int idx = Length++;
            Set(idx, value);
        }

        /// <summary>
        /// Sets all the bit values to value.
        /// </summary>
        /// <param name="value"></param>
        public void SetAll(bool value)
        {
            int arrayLength = GetInt32ArrayLengthFromBitLength(Length);
            var span = m_array.AsSpan(0, arrayLength);
            if (value)
            {
                span.Fill(-1);

                // clear high bit values in the last int
                Div32Rem(m_length, out int extraBits);
                if (extraBits > 0)
                {
                    span[^1] &= (1 << extraBits) - 1;
                }
            }
            else
            {
                span.Clear();
            }

            _version++;
        }

        public int Length
        {
            get => m_length;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value must be non-negative.");
                }

                int newints = GetInt32ArrayLengthFromBitLength(value);
                if (newints > m_array.Length || newints + _ShrinkThreshold < m_array.Length)
                {
                    // grow or shrink (if wasting more than _ShrinkThreshold ints)
                    ArrayPool<int>.Shared.Resize(ref m_array!, newints);
                }

                if (value > m_length)
                {
                    // clear high bit values in the last int
                    int last = (m_length - 1) >> BitShiftPerInt32;
                    Div32Rem(m_length, out int bits);
                    if (bits > 0)
                    {
                        m_array[last] &= (1 << bits) - 1;
                    }

                    // clear remaining int values
                    m_array.AsSpan(last + 1, newints - last - 1).Clear();
                }

                m_length = value;
                _version++;
            }
        }

        public BitArray ToBitArray()
        {
            bool[] values = new bool[Length];
            for (int i = 0; i < Length; i++)
            {
                values[i] = this[i];
            }
            BitArray result = new(values);
            return result;
        }

        public override string ToString()
            => $"Size: {Count / 8:X4}:{Count % 8:D1} ({Count / 8:D})";

        public int Count => m_length;

        public object SyncRoot => this;

        public bool IsSynchronized => false;

        public bool IsReadOnly => false;

        /// <inheritdoc cref="List.Slice" />
        public InternalBitArray Slice(int start, int length) => GetRange(start, length);

        public InternalBitArray GetRange(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Non-negative number required.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Non-negative number required.");
            }

            if (index > Count)
            {
                throw new ArgumentException("Offset were out of bounds for the array.");
            }

            if (m_length - index < count)
            {
                throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
            }

            int length = (count / BitsPerByte) + ((count % BitsPerByte == 0) ? 0 : 1);
            var bytes = new byte[length];

            for (int i = 0; i < count; i++)
            {
                int j = index + i;
                int bitIndex = i % BitsPerByte;
                int byteIndex = i / BitsPerByte;


                int mask = ((m_array[j >> 5] & (1 << j)) != 0 ? 1 : 0) << bitIndex;
                bytes[byteIndex] |= (byte)mask;
            }

            return new InternalBitArray(bytes);
        }

        public object Clone() => new InternalBitArray(this);

        public IEnumerator<bool> GetEnumerator() => new BitArrayEnumeratorSimple(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// XPerY=n means that n Xs can be stored in 1 Y.
        /// </summary>
        internal const int BitsPerInt32 = 32;
        internal const int BitsPerByte = 8;

        internal const int BitShiftPerInt32 = 5;
        internal const int BitShiftPerByte = 3;
        internal const int BitShiftForBytesPerInt32 = 2;

        /// <summary>
        /// <para>
        /// Used for conversion between different representations of bit array.
        /// </para>
        /// <para>
        /// Returns (n + (32 - 1)) / 32, rearranged to avoid arithmetic overflow.
        /// For example, in the bit to int case, the straightforward calc would
        /// be (n + 31) / 32, but that would cause overflow. So instead it's
        /// rearranged to ((n - 1) / 32) + 1.
        /// </para>
        /// <para>
        /// Due to sign extension, we don't need to special case for n == 0, if we use
        /// bitwise operations (since ((n - 1) >> 5) + 1 = 0).
        /// This doesn't hold true for ((n - 1) / 32) + 1, which equals 1.
        /// </para>
        /// 
        /// <para>
        /// Usage:
        /// </para>
        /// <para>
        /// GetArrayLength(77): returns how many ints must be
        /// allocated to store 77 bits.
        /// </para>
        /// </summary>
        /// <param name="n"></param>
        /// <returns>how many ints are required to store n bytes</returns>
        private static int GetInt32ArrayLengthFromBitLength(int n)
        {
            Debug.Assert(n >= 0);
            return (int)((uint)(n - 1 + (1 << BitShiftPerInt32)) >> BitShiftPerInt32);
        }

        private static int GetInt32ArrayLengthFromByteLength(int n)
        {
            Debug.Assert(n >= 0);
            // Due to sign extension, we don't need to special case for n == 0, since ((n - 1) >> 2) + 1 = 0
            // This doesn't hold true for ((n - 1) / 4) + 1, which equals 1.
            return (int)((uint)(n - 1 + (1 << BitShiftForBytesPerInt32)) >> BitShiftForBytesPerInt32);
        }

        internal static int GetByteArrayLengthFromBitLength(int n)
        {
            Debug.Assert(n >= 0);
            // Due to sign extension, we don't need to special case for n == 0, since ((n - 1) >> 3) + 1 = 0
            // This doesn't hold true for ((n - 1) / 8) + 1, which equals 1.
            return (int)((uint)(n - 1 + (1 << BitShiftPerByte)) >> BitShiftPerByte);
        }

        internal static int GetBitLengthFromByteArrayLength(int n)
        {
            Debug.Assert(n >= 0);
            return n * 8;
        }

        private static int Div32Rem(int number, out int remainder)
        {
            uint quotient = (uint)number / 32;
            remainder = number & (32 - 1);    // equivalent to number % 32, since 32 is a power of 2
            return (int)quotient;
        }

        private static int Div4Rem(int number, out int remainder)
        {
            uint quotient = (uint)number / 4;
            remainder = number & (4 - 1);   // equivalent to number % 4, since 4 is a power of 2
            return (int)quotient;
        }

        private static void ThrowArgumentOutOfRangeException(int index)
            => throw new ArgumentOutOfRangeException(nameof(index), index,
                $"Index was out of range. ({index / 8}:{index % 8})");

        public void Clear() => SetAll(false);

        public void Dispose()
        {
            if (m_array.Length > 0)
            {
                ArrayPool<int>.Shared.Return(m_array);
                m_array = Array.Empty<int>();
                m_length = 0;
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }


            if (array.Rank != 1)
            {
                throw new ArgumentException("Only single dimensional arrays are supported for the requested action.", nameof(array));

            }

            if (array is int[] intArray)
            {
                Div32Rem(m_length, out int extraBits);

                if (extraBits == 0)
                {
                    // we have perfect bit alignment, no need to sanitize, just copy
                    Array.Copy(m_array, 0, intArray, index, m_array.Length);
                }
                else
                {
                    int last = (m_length - 1) >> BitShiftPerInt32;
                    // do not copy the last int, as it is not completely used
                    Array.Copy(m_array, 0, intArray, index, last);

                    // the last int needs to be masked
                    intArray[index + last] = m_array[last] & unchecked((1 << extraBits) - 1);
                }
            }
            else if (array is byte[] byteArray)
            {
                int bitLength = GetBitLengthFromByteArrayLength(array.Length);

                //if ((Count - index) < bitLength)
                //{
                //    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                //}
                if (index > Count)
                {
                    throw new ArgumentException($"Offset were out of bounds for the array. ({index} vs {Count})");
                }

                for (int i = 0; i < bitLength; i++)
                {
                    int bitIndex = i % BitsPerByte;
                    int byteIndex = i / BitsPerByte;

                    int mask = (Get(index + i) ? 1 : 0) << bitIndex;
                    byteArray[byteIndex] |= (byte)mask;
                }
            }
            else if (array is bool[] boolArray)
            {
                if (array.Length - index < m_length)
                {
                    throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
                }
                uint i = 0;
                /*
                if (m_length > BitsPerInt32)
                {
                    throw new ArgumentOutOfRangeException($"Array size ({m_length}) is larger than {BitsPerInt32}");
                }
                */
                for (; i < (uint)m_length; i++)
                {
                    int elementIndex = Div32Rem((int)i, out int extraBits);
                    boolArray[(uint)index + i] = ((m_array[elementIndex] >> extraBits) & 0x00000001) != 0;
                }
            }
        }

        void ICollection<bool>.CopyTo(bool[] array, int arrayIndex)
        {
            if (array is bool[] boolArray)
            {
                CopyTo(boolArray, arrayIndex);
            }
        }

        public int IndexOf(bool item) => throw new NotImplementedException();

        public void Insert(int index, bool item) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        public bool Contains(bool item) => throw new NotImplementedException();

        public bool Remove(bool item) => throw new NotImplementedException();

        string DebuggerDisplay { get => $"Count = {m_length % 8:D} bytes"; }

        private class BitArrayDebugView(InternalBitArray bits)
        {
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public BitArrayPairs[] Bytes
            {
                get
                {
                    int bitLength = bits.m_length;
                    int size = GetByteArrayLengthFromBitLength(bitLength);
                    BitArrayPairs[] keys = new BitArrayPairs[size];

                    byte[] bytes = new byte[size];
                    bits.CopyTo(bytes, 0);
                    //return bytes;
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        keys[i] = new(bits, i, bytes[i]);
                    }
                    return keys;
                }
            }
        }

        private sealed class BitArrayEnumeratorSimple : IEnumerator<bool>, ICloneable
        {
            private readonly InternalBitArray _bitArray;
            private int _index;
            private readonly int _version;
            private bool _currentElement;

            internal BitArrayEnumeratorSimple(InternalBitArray bitArray)
            {
                _bitArray = bitArray;
                _index = -1;
                _version = bitArray._version;
            }

            public object Clone() => MemberwiseClone();

            public bool MoveNext()
            {
                if (_version != _bitArray._version)
                {
                    throw new InvalidOperationException("Enumeration failed: collection was modified.");
                }

                if (_index < (_bitArray.m_length - 1))
                {
                    _index++;
                    _currentElement = _bitArray.Get(_index);
                    return true;
                }
                else
                {
                    _index = _bitArray.m_length;
                }

                return false;
            }

            public bool Current
            {
                get
                {
                    if ((uint)_index >= (uint)_bitArray.m_length)
                    {
                        throw GetInvalidOperationException(_index);
                    }

                    return _currentElement;
                }
            }

            object IEnumerator.Current => Current;

            public void Reset()
            {
                if (_version != _bitArray._version)
                {
                    throw new InvalidOperationException("Enumeration failed: collection was modified.");
                }

                _index = -1;
            }

            private InvalidOperationException GetInvalidOperationException(int index)
            {
                if (index == -1)
                {
                    return new InvalidOperationException("Enumeration not started.");
                }
                else
                {
                    Debug.Assert(index >= _bitArray.m_length);
                    return new InvalidOperationException("Enumeration ended.");
                }
            }

            void IDisposable.Dispose() { }
        }
    }

    [DebuggerDisplay("{DebuggerDisplay,nq}", Name = "{_key,nq}")]
    internal class BitArrayPairs(InternalBitArray bits, string key, byte value)
    {
        private byte _value = value;

        public BitArrayPairs(InternalBitArray bits, int key, byte value)
            : this(bits, $"{key:X4}", value)
        {
        }

        public string Key
        {
            get { return key; }
            set
            {
                byte tempValue = ReadByte(key);
                WriteByte(key, 0);
                key = value;
                WriteByte(value, tempValue);
            }
        }

        public byte Value
        {
            get { return _value; }
            set
            {
                _value = value;
                WriteByte(key, value);
            }
        }

        string DebuggerDisplay { get => $"{_value,3:D} [0x{_value:x2} / 0b{_value:b8}]"; }

        private byte ReadByte(string offset)
            => ReadByte(int.Parse(offset, System.Globalization.NumberStyles.HexNumber));

        private byte ReadByte(int offset)
        {
            byte result = 0;
            for (int i = 0; i < InternalBitArray.BitsPerByte; i++)
            {
                int bitIndex = i % InternalBitArray.BitsPerByte;

                int mask = (bits[offset + i] == true ? 1 : 0) << bitIndex;
                result |= (byte)mask;
            }
            return result;
        }

        private void WriteByte(string offset, byte b)
            => WriteByte(int.Parse(offset, System.Globalization.NumberStyles.HexNumber), b);

        private void WriteByte(int offset, byte b)
        {
            using var _bits = new InternalBitArray(b) { Length = InternalBitArray.BitsPerByte };
            for (int i = 0; i < InternalBitArray.BitsPerByte; i++)
            {
                bits[offset + i] = _bits[i];
            }
        }
    }
}
