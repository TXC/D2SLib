using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace D2Shared.Extensions
{
    public static partial class MemoryExtensions
    {
/*
        /// <inheritdoc cref="Contains{T}(ReadOnlySpan{T}, T)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Contains<T>(this Span<T> span, T value) where T : IEquatable<T>?
        {
            if (RuntimeHelpers.IsBitwiseEquatable<T>())
            {
                if (sizeof(T) == sizeof(byte))
                {

                    return SpanHelpers.ContainsValueType(
                        ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)),
                        Unsafe.BitCast<T, byte>(value),
                        span.Length);
                }
                else if (sizeof(T) == sizeof(short))
                {
                    return SpanHelpers.ContainsValueType(
                        ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)),
                        Unsafe.BitCast<T, short>(value),
                        span.Length);
                }
                else if (sizeof(T) == sizeof(int))
                {
                    return SpanHelpers.ContainsValueType(
                        ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)),
                        Unsafe.BitCast<T, int>(value),
                        span.Length);
                }
                else if (sizeof(T) == sizeof(long))
                {
                    return SpanHelpers.ContainsValueType(
                        ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)),
                        Unsafe.BitCast<T, long>(value),
                        span.Length);
                }
            }

            return SpanHelpers.Contains(ref MemoryMarshal.GetReference(span), value, span.Length);
        }

        /// <summary>
        /// Searches for the specified value and returns true if found. If not found, returns false. Values are compared using IEquatable{T}.Equals(T).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="span">The span to search.</param>
        /// <param name="value">The value to search for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Contains<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T>?
        {
            if (RuntimeHelpers.IsBitwiseEquatable<T>())
            {
                if (sizeof(T) == sizeof(byte))
                {
                    return SpanHelpers.ContainsValueType(
                        ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(span)),
                        Unsafe.BitCast<T, byte>(value),
                        span.Length);
                }
                else if (sizeof(T) == sizeof(short))
                {
                    return SpanHelpers.ContainsValueType(
                        ref Unsafe.As<T, short>(ref MemoryMarshal.GetReference(span)),
                        Unsafe.BitCast<T, short>(value),
                        span.Length);
                }
                else if (sizeof(T) == sizeof(int))
                {
                    return SpanHelpers.ContainsValueType(
                        ref Unsafe.As<T, int>(ref MemoryMarshal.GetReference(span)),
                        Unsafe.BitCast<T, int>(value),
                        span.Length);
                }
                else if (sizeof(T) == sizeof(long))
                {
                    return SpanHelpers.ContainsValueType(
                        ref Unsafe.As<T, long>(ref MemoryMarshal.GetReference(span)),
                        Unsafe.BitCast<T, long>(value),
                        span.Length);
                }
            }

            return SpanHelpers.Contains(ref MemoryMarshal.GetReference(span), value, span.Length);
        }
*/
    }
}
