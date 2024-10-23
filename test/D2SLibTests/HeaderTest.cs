using D2Shared.Enums;
using D2SLib;
using D2SLib.Model.Save;
using FluentAssertions;
using System.IO;

namespace D2SLibTests
{
    [TestClass]
    public class HeaderTest
    {
        [ClassInitialize]
        public static void InitialSetUp(TestContext ctx)
        {
            Core.Importer.LoadData();
        }

        [TestMethod]
        [DataRow(SaveVersion.v200, "Amazon")]
        [DataRow(SaveVersion.v200, "Assassin")]
        [DataRow(SaveVersion.v200, "Barbarian")]
        [DataRow(SaveVersion.v200, "Druid")]
        [DataRow(SaveVersion.v200, "Necromancer")]
        [DataRow(SaveVersion.v200, "Paladin")]
        [DataRow(SaveVersion.v200, "Sorceress")]
        [DataRow(SaveVersion.v200, "DannyIsGreat")]
        [DataRow(SaveVersion.v11x, "simple")]
        [DataRow(SaveVersion.v11x, "complex")]
        [DataRow(SaveVersion.v240, "Agelatus")]
        [DataRow(SaveVersion.v240, "WatahaWpierdala")]
        [DataRow(SaveVersion.v240, "PaladinTwo")]
        [DataRow(SaveVersion.v250, "Wilhelm")]
        [DataRow(SaveVersion.v250, "Assassin")]
        public void AllHeadersShouldBeCorrect(SaveVersion Version, string CharacterName)
        {
            D2S character = Core.ReadD2S(File.ReadAllBytes(@$"Resources/chars/{(int)Version}/{CharacterName}.d2s"));
            character.Header.Magic.Should().Be(0xaa55aa55);
            if (character.Quests.Header is null)
            {
                return;
            }
            character.Quests.Header.Should().Be(0x216f6f57);            // Woo!
            character.Waypoints.Header.Should().Be(0x5357);             // WS
            character.NPCDialog.Header.Should().Be(0x7701);             // 
            character.Attributes.Header.Should().Be(0x6667);            // gf
            character.ClassSkills.Header.Should().Be(0x6669);           // if
            character.PlayerItemList.Header.Should().Be(0x4d4a);        // JM
            character.PlayerCorpses.Header.Should().Be(0x4d4a);         // JM
            if (character.Status.IsExpansion)
            {
                character.MercenaryItemList.Header.Should().Be(0x666a); // jf
                character.Golem.Header.Should().Be(0x666b);             // kf
            }
        }

        [TestMethod]
        [DataRow(SaveVersion.v240, "InitialSave")]
        public void InitialSaveShouldBeHandled(SaveVersion Version, string CharacterName)
        {
            D2S character = Core.ReadD2S(File.ReadAllBytes(@$"Resources/chars/{(int)Version}/{CharacterName}.d2s"));
            character.Header.Magic.Should().Be(0xaa55aa55);
            character.Quests.Header.Should().BeNull();
            character.Waypoints.Header.Should().BeNull();
            character.NPCDialog.Header.Should().BeNull();
            character.Attributes.Header.Should().BeNull();
            character.ClassSkills.Header.Should().BeNull();
            character.PlayerItemList.Header.Should().BeNull();
            character.PlayerCorpses.Header.Should().BeNull();
            if (character.Status.IsExpansion)
            {
                character.MercenaryItemList.Header.Should().BeNull();
                character.Golem.Header.Should().BeNull();
            }
        }
    }
}
