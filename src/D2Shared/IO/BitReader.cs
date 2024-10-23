using CommunityToolkit.HighPerformance.Buffers;
using D2Shared.Extensions;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Buffers.Binary;

namespace D2Shared.IO
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public sealed class BitReader : IBitReader, IDisposable
    {
        private const int STACK_MAX = 0xff;

        private InternalBitArray _bits;
        public int Position { get; private set; }

        public BitReader(ReadOnlySpan<byte> bytes)
        {
            Position = 0;
            _bits = new InternalBitArray(bytes);
        }

        public void Align() => Position = (Position + 7) & ~7;

        #region Read
        public bool ReadBit() => _bits[Position++];

        public byte[] ReadBits(int numberOfBits)
        {
            byte[] bytes = new byte[InternalBitArray.GetByteArrayLengthFromBitLength(numberOfBits)];
            if (ReadBits(numberOfBits, bytes) > 0)
            {
                return bytes;
            }
            return [];
        }

        public int ReadBits(int numberOfBits, Span<byte> output)
        {
            int byteCount = InternalBitArray.GetByteArrayLengthFromBitLength(numberOfBits);

            if (output.Length < byteCount)
            {
                throw new ArgumentOutOfRangeException(nameof(output));
            }
            try
            {
                for (int i = 0; i < numberOfBits; i++)
                {
                    int bitIndex = i % InternalBitArray.BitsPerByte;
                    int byteIndex = i / InternalBitArray.BitsPerByte;

                    int mask = (_bits[Position + i] == true ? 1 : 0) << bitIndex;
                    output[byteIndex] |= (byte)mask;
                }
                /*
                int byteIndex = 0;
                int bitIndex = 0;
                for (int i = 0; i < numberOfBits; i++)
                {
                    if (_bits[Position + i])
                    {
                        output[byteIndex] |= (byte)(1 << bitIndex);
                    }
                    bitIndex++;
                    if (bitIndex == 8)
                    {
                        byteIndex++;
                        bitIndex = 0;
                    }
                }
                */
                Position += numberOfBits;
            }
            catch (ArgumentOutOfRangeException)
            {
                return -1;
            }

            return byteCount;
        }

        public MemoryOwner<byte> ReadBitsPooled(int numberOfBits)
        {
            var bytes = MemoryOwner<byte>.Allocate(InternalBitArray.GetByteArrayLengthFromBitLength(numberOfBits));
            if (ReadBits(numberOfBits, bytes.Span) > 0)
            {
                return bytes;
            }
            return bytes;
        }

        IMemoryOwner<byte> IBitReader.ReadBitsPooled(int numberOfBits) => ReadBitsPooled(numberOfBits);

        public int? ReadBytes(int numberOfBytes, Span<byte> output) => ReadBits(numberOfBytes * 8, output);

        public int? ReadBytes(Span<byte> output) => ReadBits(output.Length * 8, output);

        public byte[] ReadBytes(int numberOfBytes) => ReadBits(numberOfBytes * 8);

        public MemoryOwner<byte> ReadBytesPooled(int numberOfBytes) => ReadBitsPooled(numberOfBytes * 8);

        IMemoryOwner<byte> IBitReader.ReadBytesPooled(int numberOfBytes) => ReadBitsPooled(numberOfBytes * 8);

        public byte? ReadByte(int bits)
        {
            if ((uint)bits > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }
            Span<byte> bytes = stackalloc byte[1];
            bytes.Clear();
            if (ReadBits(bits, bytes) > 0)
            {
                return bytes[0];
            }
            return null;
        }

        public byte? ReadByte() => ReadByte(8);

        public short? ReadInt16(int bits)
        {
            if ((uint)bits > sizeof(short) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }
            Span<byte> bytes = stackalloc byte[sizeof(short)];
            bytes.Clear();
            if (ReadBits(bits, bytes) > 0)
            {
                return BinaryPrimitives.ReadInt16LittleEndian(bytes);
            }
            return null;
        }

        public short? ReadInt16() => ReadInt16(sizeof(short) * 8);

        public ushort? ReadUInt16(int bits)
        {
            if ((uint)bits > sizeof(ushort) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }

            Span<byte> bytes = stackalloc byte[sizeof(ushort)];
            bytes.Clear();
            if (ReadBits(bits, bytes) > 0)
            {
                return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
            }
            return null;
        }

        public ushort? ReadUInt16() => ReadUInt16(sizeof(ushort) * 8);

        public uint? ReadUInt32(int bits)
        {
            if ((uint)bits > sizeof(uint) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }

            Span<byte> bytes = stackalloc byte[sizeof(uint)];
            bytes.Clear();
            if (ReadBits(bits, bytes) > 0)
            {
                return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
            }
            return null;
        }

        public uint? ReadUInt32() => ReadUInt32(sizeof(uint) * 8);

        public int? ReadInt32(int bits)
        {
            if ((uint)bits > sizeof(int) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }

            Span<byte> bytes = stackalloc byte[sizeof(int)];
            bytes.Clear();
            if (ReadBits(bits, bytes) > 0)
            {
                return BinaryPrimitives.ReadInt32LittleEndian(bytes);
            }
            return null;
        }

        public int? ReadInt32() => ReadInt32(sizeof(int) * 8);

        public string ReadString(int byteCount)
        {
            using var pooledBytes = byteCount > STACK_MAX ? SpanOwner<byte>.Allocate(byteCount) : SpanOwner<byte>.Empty;
            Span<byte> bytes = byteCount > STACK_MAX ? pooledBytes.Span : stackalloc byte[byteCount];
            bytes.Clear();
            int? readBytes = ReadBytes(bytes);
            if (readBytes.HasValue)
            {
                bytes = bytes[..readBytes.Value];
            }
            return Encoding.ASCII.GetString(bytes.Trim((byte)0));
        }
        #endregion Read

        #region Advance
        public void AdvanceBits(int bits)
        {
            if ((Position + bits) <= _bits.Length)
            {
                Position += bits;
            }
            else
            {
                Position = _bits.Length;
            }
        }
        public void Advance(int bytes) => AdvanceBits(bytes * 8);
        #endregion Advance

        #region Seek
        public void SeekBits(int bitPosition)
        {
            if (bitPosition <= _bits.Length)
            {
                Position = bitPosition;
            }
            else
            {
                Position = _bits.Length;
            }
        }

        public void Seek(int bytePostion) => SeekBits(bytePostion * 8);
        #endregion Seek

        #region Peek
        public bool PeekBit() => _bits[Position + 1];

        public byte[] PeekBits(int numberOfBits)
        {
            byte[] bytes = new byte[InternalBitArray.GetByteArrayLengthFromBitLength(numberOfBits)];
            if (PeekBits(numberOfBits, bytes) > 0)
            {
                return bytes;
            }
            return [];
        }

        private int PeekBits(int numberOfBits, Span<byte> output)
        {
            int byteCount = InternalBitArray.GetByteArrayLengthFromBitLength(numberOfBits);

            if (output.Length < byteCount)
            {
                throw new ArgumentOutOfRangeException(nameof(output));
            }
            try
            {
                for (int i = 0; i < numberOfBits; i++)
                {
                    int bitIndex = i % InternalBitArray.BitsPerByte;
                    int byteIndex = i / InternalBitArray.BitsPerByte;

                    int mask = (_bits[Position + i] == true ? 1 : 0) << bitIndex;
                    output[byteIndex] |= (byte)mask;
                }
                //Position += numberOfBits;
            }
            catch (ArgumentOutOfRangeException)
            {
                return -1;
            }

            return byteCount;
        }

        public byte? PeekByte() => PeekByte(8);

        public byte? PeekByte(int bits)
        {
            if ((uint)bits > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }

            Span<byte> bytes = stackalloc byte[1];
            bytes.Clear();
            int bytesRead = PeekBits(bits, bytes);
            if (bytesRead == -1)
            {
                return null;
            }
            return bytes[0];
        }

        public short? PeekInt16() => PeekInt16(sizeof(short) * 8);

        public short? PeekInt16(int bits)
        {
            if ((uint)bits > sizeof(short) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }
            Span<byte> bytes = stackalloc byte[sizeof(short)];
            bytes.Clear();
            int bytesRead = PeekBits(bits, bytes);
            if (bytesRead == -1)
            {
                return null;
            }
            return BinaryPrimitives.ReadInt16LittleEndian(bytes);
        }

        public ushort? PeekUInt16() => PeekUInt16(sizeof(ushort) * 8);

        public ushort? PeekUInt16(int bits)
        {
            if ((uint)bits > sizeof(ushort) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }
            Span<byte> bytes = stackalloc byte[sizeof(ushort)];
            bytes.Clear();
            int bytesRead = PeekBits(bits, bytes);
            if (bytesRead == -1)
            {
                return null;
            }
            return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
        }

        public int? PeekInt32() => PeekInt32(sizeof(int) * 8);

        public int? PeekInt32(int bits)
        {
            if ((uint)bits > sizeof(int) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }
            Span<byte> bytes = stackalloc byte[sizeof(int)];
            bytes.Clear();
            int bytesRead = PeekBits(bits, bytes);
            if (bytesRead == -1)
            {
                return null;
            }
            return BinaryPrimitives.ReadInt32LittleEndian(bytes);
        }

        public uint? PeekUInt32() => PeekUInt32(sizeof(uint) * 8);

        public uint? PeekUInt32(int bits)
        {
            if ((uint)bits > sizeof(uint) * 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bits));
            }
            Span<byte> bytes = stackalloc byte[sizeof(uint)];
            bytes.Clear();
            int bytesRead = PeekBits(bits, bytes);
            if (bytesRead == -1)
            {
                return null;
            }
            return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
        }
        #endregion Peek

        #region Get
        public byte? GetByte(int position)
        {
            int _position = Position;
            Seek(position);
            var result = PeekByte();
            Position = _position;
            return result;
        }

        public short? GetInt16(int position)
        {
            int _position = Position;
            Seek(position);
            var result = PeekInt16();
            Position = _position;
            return result;
        }

        public ushort? GetUInt16(int position)
        {
            int _position = Position;
            Seek(position);
            var result = PeekUInt16();
            Position = _position;
            return result;
        }

        public int? GetInt32(int position)
        {
            int _position = Position;
            Seek(position);
            var result = PeekInt32();
            Position = _position;
            return result;
        }

        public uint? GetUInt32(int position)
        {
            int _position = Position;
            Seek(position);
            var result = PeekUInt32();
            Position = _position;
            return result;
        }
        #endregion Get

        public void Dispose() => Interlocked.Exchange(ref _bits!, null)?.Dispose();

        string DebuggerDisplay { get => $"Position: {Position / 8:D} (${Position / 8:X4}:{Position % 8:D1})"; }
    }
}
