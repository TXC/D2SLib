using D2SLib.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
//using System.IO;
using System.Text;
using static System.Buffers.Binary.BinaryPrimitives;

namespace D2SLibTests;

[TestClass]
public sealed class BitReaderTests
{
    [TestMethod]
    public void CanReadBits()
    {
        byte[] bytes = new byte[32];
        new Random(1337).NextBytes(bytes);
        using var bro = new BitReader_Old(bytes);
        using var br = new BitReader(bytes);

        var oldBits = bro.ReadBits(17);
        var newBits = br.ReadBits(17);

        for (int i = 0; i < oldBits.Length; i++)
        {
            Console.Write(Convert.ToString(oldBits[i], 2).PadLeft(8, '0'));
            Console.Write(' ');
        }
        Console.WriteLine();

        for (int i = 0; i < newBits.Length; i++)
        {
            Console.Write(Convert.ToString(newBits[i], 2).PadLeft(8, '0'));
            Console.Write(' ');
        }
        Console.WriteLine();

        oldBits.Should().BeEquivalentTo(newBits);
        bro.Position.Should().Be(br.Position);
    }

    [TestMethod]
    public void CanReadByte()
    {
        byte[] bytes = [137];
        using var bro = new BitReader_Old(bytes);
        using var br = new BitReader(bytes);

        bro.ReadByte().Should().Be(br.ReadByte());
        bro.Position.Should().Be(br.Position);
    }

    [TestMethod]
    public void CanReadBytes()
    {
        byte[] bytes = new byte[97];
        new Random(1337).NextBytes(bytes);
        using var bro = new BitReader_Old(bytes);
        using var br = new BitReader(bytes);

        bro.SeekBits(5);
        br.SeekBits(5);

        var oldBits = bro.ReadBytes(95);
        var newBits = br.ReadBytes(95);

        oldBits.Should().BeEquivalentTo(newBits);
        bro.Position.Should().Be(br.Position);
    }

    [TestMethod]
    public void CanReadInt32()
    {
        byte[] bytes = new byte[sizeof(int)];
        WriteInt32LittleEndian(bytes, 1370048);
        using var bro = new BitReader_Old(bytes);
        using var br = new BitReader(bytes);

        bro.ReadInt32().Should().Be(br.ReadInt32());
        bro.Position.Should().Be(br.Position);
    }

    [TestMethod]
    public void CanReadUInt32()
    {
        byte[] bytes = new byte[sizeof(uint)];
        WriteUInt32LittleEndian(bytes, 1370048);
        using var bro = new BitReader_Old(bytes);
        using var br = new BitReader(bytes);

        bro.ReadUInt32().Should().Be(br.ReadUInt32());
        bro.Position.Should().Be(br.Position);
    }

    [TestMethod]
    public void CanReadUInt16()
    {
        byte[] bytes = new byte[sizeof(ushort)];
        WriteUInt16LittleEndian(bytes, 7401);
        using var bro = new BitReader_Old(bytes);
        using var br = new BitReader(bytes);

        bro.ReadUInt16().Should().Be(br.ReadUInt16());
        bro.Position.Should().Be(br.Position);
    }

    [TestMethod]
    public void CanReadString()
    {
        byte[] bytes = Encoding.ASCII.GetBytes("test");
        using var bro = new BitReader_Old(bytes);
        using var br = new BitReader(bytes);

        bro.ReadString(4).Should().Be(br.ReadString(4));
        bro.Position.Should().Be(br.Position);
    }
}
