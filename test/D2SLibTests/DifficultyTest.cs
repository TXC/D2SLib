using D2Shared.IO;
using D2SImporter;
using D2SLib.Model.Save;
using FluentAssertions;

namespace D2SLibTests;

[TestClass]
public class DifficultyTest
{
    FromAssemblyImporter Importer = new();

    /*
    [ClassInitialize]
    public static void InitialSetUp(TestContext ctx)
    {
        Core.Importer.LoadData();
    }
    */

    [TestMethod]
    [DataRow(0x00, false, 0)]
    [DataRow(0x81, true, 1)]
    [DataRow(0x82, true, 2)]
    [DataRow(0x83, true, 3)]
    [DataRow(0x84, true, 4)]
    [DataRow(0x85, true, 5)]
    public void VerifyDifficultyRead(int value, bool active, int act)
    {
        using var reader = new BitReader([(byte)value]);
        var diff = Difficulty.Read(reader);

        ValidateDifficulty(diff, active, (byte)act);

        using var writer = new BitWriter();
        diff.Write(writer);
        byte[] bytes = writer.ToArray();

        bytes.Should().BeEquivalentTo([value]);
    }

    [TestMethod]
    [DataRow(new byte[3] { 0x00, 0x00, 0x00 }, false, 0)]
    [DataRow(new byte[3] { 0x81, 0x00, 0x00 }, true, 1)]
    [DataRow(new byte[3] { 0x82, 0x00, 0x00 }, true, 2)]
    [DataRow(new byte[3] { 0x83, 0x00, 0x00 }, true, 3)]
    [DataRow(new byte[3] { 0x84, 0x00, 0x00 }, true, 4)]
    [DataRow(new byte[3] { 0x85, 0x00, 0x00 }, true, 5)]
    public void VerifyNormalDifficultyRead(byte[] value, bool active, int act)
    {
        using var reader = new BitReader(value);
        var diff = Difficulties.Read(reader);

        ValidateDifficulty(diff.Normal, active, (byte)act);
        ValidateDifficulty(diff.Nightmare, false, 0);
        ValidateDifficulty(diff.Hell, false, 0);

        using var writer = new BitWriter();
        diff.Write(writer);
        byte[] bytes = writer.ToArray();

        bytes.Should().BeEquivalentTo(value);
    }

    [TestMethod]
    [DataRow(new byte[3] { 0x00, 0x00, 0x00 }, false, 0)]
    [DataRow(new byte[3] { 0x00, 0x81, 0x00 }, true, 1)]
    [DataRow(new byte[3] { 0x00, 0x82, 0x00 }, true, 2)]
    [DataRow(new byte[3] { 0x00, 0x83, 0x00 }, true, 3)]
    [DataRow(new byte[3] { 0x00, 0x84, 0x00 }, true, 4)]
    [DataRow(new byte[3] { 0x00, 0x85, 0x00 }, true, 5)]
    public void VerifyNightmareDifficultyRead(byte[] value, bool active, int act)
    {
        using var reader = new BitReader(value);
        var diff = Difficulties.Read(reader);

        ValidateDifficulty(diff.Normal, false, 0);
        ValidateDifficulty(diff.Nightmare, active, (byte)act);
        ValidateDifficulty(diff.Hell, false, 0);

        using var writer = new BitWriter();
        diff.Write(writer);
        byte[] bytes = writer.ToArray();

        bytes.Should().HaveCount(3)
                  .And.BeEquivalentTo(value);
    }

    [TestMethod]
    [DataRow(new byte[3] { 0x00, 0x00, 0x00 }, false, 0)]
    [DataRow(new byte[3] { 0x00, 0x00, 0x81 }, true, 1)]
    [DataRow(new byte[3] { 0x00, 0x00, 0x82 }, true, 2)]
    [DataRow(new byte[3] { 0x00, 0x00, 0x83 }, true, 3)]
    [DataRow(new byte[3] { 0x00, 0x00, 0x84 }, true, 4)]
    [DataRow(new byte[3] { 0x00, 0x00, 0x85 }, true, 5)]
    public void VerifyHellDifficultyRead(byte[] value, bool active, int act)
    {
        using var reader = new BitReader(value);
        var diff = Difficulties.Read(reader);

        ValidateDifficulty(diff.Normal, false, 0);
        ValidateDifficulty(diff.Nightmare, false, 0);
        ValidateDifficulty(diff.Hell, active, (byte)act);

        using var writer = new BitWriter();
        diff.Write(writer);
        byte[] bytes = writer.ToArray();

        bytes.Should().HaveCount(3)
                  .And.BeEquivalentTo(value);
    }

    private static void ValidateDifficulty(Difficulty difficulty, bool active, byte act)
    {
        if (active)
        {
            difficulty.Active.Should().BeTrue();
        }
        else
        {
            difficulty.Active.Should().BeFalse();
        }
        difficulty.Act.Should().Be(act);
        difficulty.Flags[3].Should().BeFalse();
        difficulty.Flags[4].Should().BeFalse();
        difficulty.Flags[5].Should().BeFalse();
        difficulty.Flags[6].Should().BeFalse();
    }
}
