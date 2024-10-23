using D2Shared.Enums;
using D2SLib;
using D2SLib.Model.Save;
using FluentAssertions;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace D2SLibTests;

[TestClass]
public class D2STest
{
    static readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [ClassInitialize]
    public static void InitialSetUp(TestContext ctx)
    {
        Core.Importer.LoadData();
    }

    [TestInitialize]
    public void SetUp()
    {
        //Core.Importer.LoadFromAssembly();
    }

    [ClassCleanup]
    public static void FinalTearDown()
    {
        //Core.Importer.LoadFromAssembly();
    }

    [TestCleanup]
    public void TearDown()
    {
        //Core.Importer.LoadFromAssembly();
    }

    [TestMethod, TestCategory("v2.00")]
    [DataRow(CharacterClass.Amazon)]
    [DataRow(CharacterClass.Assassin)]
    [DataRow(CharacterClass.Barbarian)]
    [DataRow(CharacterClass.Druid)]
    [DataRow(CharacterClass.Necromancer)]
    [DataRow(CharacterClass.Paladin)]
    [DataRow(CharacterClass.Sorceress)]
    public void ShouldReadVersion97SimpleCharacter(CharacterClass ClassId)
    {
        string Name = Enum.GetName(typeof(CharacterClass), ClassId);

        D2S character = Core.ReadD2S(File.ReadAllBytes(@$"Resources/chars/{(int)SaveVersion.v200}/{Name}.d2s"));
        character.Name.Should().Be(Name);
        character.ClassId.Should().Be(ClassId);
        character.Header.Version.Should().Be(SaveVersion.v200);

        LogCharacter(character);
    }

    [TestMethod, TestCategory("v2.00")]
    public void ShouldReadVersion97ComplexCharacter()
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(@"Resources/chars/97/DannyIsGreat.d2s"));
        character.Name.Should().Be("DannyIsGreat");
        character.ClassId.Should().Be(CharacterClass.Sorceress);

        LogCharacter(character);
    }

    [TestMethod, TestCategory("v2.00")]
    public void ShouldWriteVersion97ComplexCharacter()
    {
        byte[] input = File.ReadAllBytes(@"Resources/chars/97/DannyIsGreat.d2s");
        D2S baseCharacter = D2S.Read(input);
        byte[] ret = D2S.Write(baseCharacter);

        D2S character = D2S.Read(ret);

        character.Name.Should().Be("DannyIsGreat");
        character.ClassId.Should().Be(CharacterClass.Sorceress);

        LogCharacter(character);
    }

    [TestMethod, TestCategory("v2.00")]
    public void ShouldCalculateChecksum()
    {
        var bytes = File.ReadAllBytes(@"Resources/chars/96/simple.d2s");
        var pre = bytes[0xc..0x10].ToList();
        Header.Fix(bytes);
        var post = bytes[0xc..0x10].ToList();

        post.Should().NotBeEmpty()
            .And.HaveCount(4)
            .And.BeEquivalentTo(pre);
    }

    [TestMethod]
    [TestCategory("v2.40")]
    [DataRow("Agelatus", 81, 54)]
    [DataRow("WatahaWpierdala", 75, 39)]
    [DataRow("PaladinTwo", 159, 73)]
    public void ShouldReadVersion98ComplexCharacter(string name, int strength, int noOfItems)
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(@$"Resources/chars/{(int)SaveVersion.v240}/{name}.d2s"));
        character.Name.Should().Be(name);
        character.Header.Version.Should().Be(SaveVersion.v240);
        character.Attributes.Stats.Should().Contain("strength", strength);
        character.PlayerItemList.Items.Should().HaveCount(noOfItems);
    }

    [TestMethod, TestCategory("v2.40")]
    public void ShouldWriteVersion98ComplexCharacter()
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(@$"Resources/chars/{(int)SaveVersion.v240}/Agelatus.d2s"));
        byte[] ret = Core.WriteD2S(character);
        ret.Length.Should().Be(2684);
        File.WriteAllBytes(@$"Resources/D2STest{(int)SaveVersion.v240}Agelatus.d2s", ret);
        D2S again = D2S.Read(ret);

        again.Should().BeEquivalentTo(character);
        //again.Header.Checksum.Should().Be(character.Header.Checksum);
    }

    [TestMethod, TestCategory("v2.40")]
    public void ShouldReadVersion98NewCharacter()
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(path: @$"Resources/chars/{(int)SaveVersion.v240}/InitialSave.d2s"));
        character.Name.Should().Be("InitialSave");
        character.Attributes.Stats.Should().Contain("strength", 30);
    }

    [TestMethod, TestCategory("v2.50")]
    [DataRow("Wilhelm")]
    [DataRow("Assassin")]
    public void ShouldReadVersion99Character(string name)
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(path: @$"Resources/chars/{(int)SaveVersion.v250}/{name}.d2s"));
        character.Name.Should().Be(name);
        //character.Attributes.Stats.Should().Contain("strength", 30);
    }

    [TestMethod, TestCategory("v2.50")]
    public void ShouldReadVersion99CharacterAutoDetect()
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(path: @$"Resources/chars/{(int)SaveVersion.v250}/Wilhelm.d2s"));
        character.Name.Should().Be("Wilhelm");
    }

    [TestMethod, TestCategory("v1.14d")]
    public void ShouldReadVersion96NewCharacter()
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(path: @$"Resources/chars/{(int)SaveVersion.v11x}/simple.d2s"));
        character.Name.Should().Be("Simple");
        character.Attributes.Stats.Should().Contain("strength", 30);
    }

    [TestMethod, TestCategory("v1.14d")]
    public void ShouldWriteVersion96NewCharacter()
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(path: @$"Resources/chars/{(int)SaveVersion.v11x}/simple.d2s"));
        character.Name.Should().Be("Simple");
        byte[] ret = Core.WriteD2S(character);
        ret.Length.Should().Be(998);
    }

    [TestMethod, TestCategory("v1.14d")]
    public void ShouldReadVersion96ComplexCharacter()
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(path: @$"Resources/chars/{(int)SaveVersion.v11x}/complex.d2s"));
        character.Name.Should().Be("Complex");
        character.PlayerItemList.Items.Should().HaveCount(61);
    }

    [TestMethod, TestCategory("v1.14d")]
    public void ShouldWriteVersion96ComplexCharacter()
    {
        D2S character = Core.ReadD2S(File.ReadAllBytes(path: @$"Resources/chars/{(int)SaveVersion.v11x}/complex.d2s"));
        character.Name.Should().Be("Complex");
        byte[] ret = Core.WriteD2S(character);
        //ret.Length.Should().Be(3244);
        ret.Length.Should().Be(3196);
    }

    [Conditional("DEBUG")]
    private static void LogCharacter(D2S character, string? label = null)
    {
        if (label is not null)
        {
            Console.Write(label);
            Console.WriteLine(':');
        }

        Console.WriteLine(JsonSerializer.Serialize(character, jsonOptions));
    }
}
