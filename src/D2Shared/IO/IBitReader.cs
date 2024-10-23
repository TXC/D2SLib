using System;
using System.Buffers;

namespace D2Shared.IO
{

    public interface IBitReader
    {
        int Position { get; }

        void Align();
        bool ReadBit();
        byte[] ReadBits(int numberOfBits);
        IMemoryOwner<byte> ReadBitsPooled(int numberOfBits);
        int ReadBits(int numberOfBits, Span<byte> output);
        byte? ReadByte();
        byte? ReadByte(int bits);
        byte[] ReadBytes(int numberOfBytes);
        IMemoryOwner<byte> ReadBytesPooled(int numberOfBytes);
        int? ReadBytes(int numberOfBytes, Span<byte> output);
        int? ReadBytes(Span<byte> output);
        int? ReadInt32();
        int? ReadInt32(int bits);
        short? ReadInt16();
        short? ReadInt16(int bits);
        string ReadString(int byteCount);
        ushort? ReadUInt16();
        ushort? ReadUInt16(int bits);
        uint? ReadUInt32();
        uint? ReadUInt32(int bits);
        void Seek(int bytePostion);
        void SeekBits(int bitPosition);
        void AdvanceBits(int bits);
        void Advance(int bytes);

        bool PeekBit();
        byte[] PeekBits(int numberOfBits);
        byte? PeekByte();
        byte? PeekByte(int bits);
        short? PeekInt16();
        short? PeekInt16(int bits);
        ushort? PeekUInt16();
        ushort? PeekUInt16(int bits);
        int? PeekInt32();
        int? PeekInt32(int bits);
        uint? PeekUInt32();
        uint? PeekUInt32(int bits);

        //byte[] GetBits(int position);

        byte? GetByte(int position);

        int? GetInt32(int position);

        short? GetInt16(int position);

        ushort? GetUInt16(int position);

        uint? GetUInt32(int position);
    }
}
