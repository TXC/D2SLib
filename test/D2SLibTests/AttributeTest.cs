using D2SLib;
using D2SImporter;
using D2Shared.IO;
using D2SLib.Model.Save;
using FluentAssertions;
using System;
using System.Text;

namespace D2SLibTests;

[TestClass]
public class AttributeTest
{
    FromAssemblyImporter Importer = new();
    [ClassInitialize]
    public static void InitialSetUp(TestContext ctx)
    {
        Core.Importer.LoadData();
    }
    /*
    [TestMethod]
    public void VerifyNPCDialogFromFile()
    {
        var values = File.ReadAllBytes(@$"Resources/chars/{(int)SaveVersion.v240}/Agelatus.d2s");
        D2S character = Core.ReadD2S(values);
        byte[] ret = Core.WriteD2S(character);
        ret[0x2c9..0x2fd].Should().BeEquivalentTo(values[0x2c9..0x2fd]);
    }
    */

    [TestMethod]
    public void TestAttributeRead()
    {
        byte[] bytes = {
            0x67, 0x66, 0x00, 0x14, 0x08, 0x30, 0x82, 0x80, 0x0C, 0x06, 0x28, 0x40,
            0xE0, 0xFF, 0x02, 0xF9, 0x06, 0xFE, 0xFF, 0xFF, 0x01, 0x00, 0x14, 0x80,
            0xE0, 0xFF, 0xFF, 0x27, 0x00, 0x18, 0x01, 0x0A, 0x00, 0xA8, 0xC9, 0x02,
            0x00, 0x25, 0xC0, 0x60, 0xDC, 0xC0, 0xF0, 0xCA, 0x3A, 0xDA, 0x01, 0xCC,
            0xC6, 0x83, 0x07, 0xA0, 0x25, 0x26, 0xFE, 0x03
        };

        using var reader = new BitReader(bytes);

        var attributes = Attributes.Read(reader);
        attributes.Header.Should().NotBeNull();

        var headerBytes = BitConverter.GetBytes((ushort)attributes.Header);
        var header = Encoding.ASCII.GetString(headerBytes);
        header.Should().Be("gf");

        attributes.Stats.Should().HaveCount(16);
    }
}
