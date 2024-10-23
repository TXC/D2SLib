using D2Shared.Enums;
using D2Shared.IO;
using D2SImporter;
using D2SLib;
using D2SLib.Model.Save;
using FluentAssertions;
using System;
using System.IO;

namespace D2SLibTests;

[TestClass]
public class NPCDialogTest
{
    FromAssemblyImporter Importer = new();
    [ClassInitialize]
    public static void InitialSetUp(TestContext ctx)
    {
        Core.Importer.LoadData();
    }

    [TestMethod]
    public void VerifyNPCDialogFromFile()
    {
        var values = File.ReadAllBytes(@$"Resources/chars/{(int)SaveVersion.v240}/Agelatus.d2s");
        D2S character = Core.ReadD2S(values);
        byte[] ret = Core.WriteD2S(character);
        ret[0x2c9..0x2fd].Should().BeEquivalentTo(values[0x2c9..0x2fd]);
    }

    [TestMethod, Ignore]
    //[DynamicData(nameof(AdditionData))]
    public void VerifyNPCDialogRead()
    {
        Span<byte> values = [
            0x01, 0x77, 0x34, 0x00,                         // Header (4 bytes)
            0xAE, 0xBE, 0xA5, 0xD9, 0x07, 0x00, 0x00, 0x00, // Introductions Normal (8 bytes) $00
            0xAE, 0xBE, 0xA5, 0xD9, 0x07, 0x00, 0x00, 0x00, // Introductions Nightmare (8 bytes) $08
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Introductions Hell (8 bytes) $10
            0x82, 0xFB, 0xE1, 0x18, 0x00, 0x00, 0x00, 0x00, // Congratulated Normal (8 bytes) $18
            0xDA, 0x40, 0x83, 0x18, 0x00, 0x00, 0x00, 0x00, // Congratulated Nightmare (8 bytes) $20
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Congratulated Hell (8 bytes) $28
        ];

        using var reader = new BitReader(values);
        var dialogs = NPCDialogSection.Read(reader);

        dialogs.Should().NotBeNull();

        ValidateNormalDifficulty(dialogs.Normal);
        ValidateNightmareDifficulty(dialogs.Nightmare);
        ValidateHellDifficulty(dialogs.Hell);

        using var writer = new BitWriter();
        dialogs.Write(writer);
        byte[] bytes = writer.ToArray();

        bytes.Should().BeEquivalentTo(values.ToArray());
    }

    private static void ValidateNormalDifficulty(NPCDialogDifficulty dialog)
    {
        dialog.Should().NotBeNull();

        #region Introductions
        dialog.WarrivActII.Introduction.Should().BeFalse();
        dialog.Charsi.Introduction.Should().BeTrue();
        dialog.WarrivActI.Introduction.Should().BeTrue();
        dialog.Kashya.Introduction.Should().BeFalse();
        dialog.Akara.Introduction.Should().BeTrue();
        dialog.Gheed.Introduction.Should().BeFalse();

        dialog.Greiz.Introduction.Should().BeFalse();
        dialog.Jerhyn.Introduction.Should().BeTrue();
        dialog.MeshifActII.Introduction.Should().BeTrue();
        dialog.Geglash.Introduction.Should().BeTrue();
        dialog.Lysander.Introduction.Should().BeTrue();
        dialog.Fara.Introduction.Should().BeTrue();
        dialog.Drogan.Introduction.Should().BeFalse();

        dialog.Alkor.Introduction.Should().BeTrue();
        dialog.Hratli.Introduction.Should().BeFalse();
        dialog.Ashera.Introduction.Should().BeTrue();
        dialog.CainActIII.Introduction.Should().BeTrue();
        dialog.Elzix.Introduction.Should().BeTrue();

        dialog.Malah.Introduction.Should().BeTrue();
        dialog.Anya.Introduction.Should().BeFalse();
        dialog.Natalya.Introduction.Should().BeTrue();
        dialog.MeshifActIII.Introduction.Should().BeTrue();
        dialog.Ormus.Introduction.Should().BeTrue();

        dialog.CainActV.Introduction.Should().BeFalse();
        dialog.Qualkehk.Introduction.Should().BeFalse();
        dialog.Nihlathak.Introduction.Should().BeFalse();
        #endregion Introductions

        #region Congratulations
        dialog.WarrivActII.Congratulations.Should().BeFalse();
        dialog.Charsi.Congratulations.Should().BeFalse();
        dialog.WarrivActI.Congratulations.Should().BeFalse();
        dialog.Kashya.Congratulations.Should().BeFalse();
        dialog.Akara.Congratulations.Should().BeFalse();
        dialog.Gheed.Congratulations.Should().BeFalse();

        dialog.Greiz.Congratulations.Should().BeTrue();
        dialog.Jerhyn.Congratulations.Should().BeTrue();
        dialog.MeshifActII.Congratulations.Should().BeFalse();
        dialog.Geglash.Congratulations.Should().BeTrue();
        dialog.Lysander.Congratulations.Should().BeTrue();
        dialog.Fara.Congratulations.Should().BeTrue();
        dialog.Drogan.Congratulations.Should().BeTrue();

        dialog.Alkor.Congratulations.Should().BeTrue();
        dialog.Hratli.Congratulations.Should().BeFalse();
        dialog.Ashera.Congratulations.Should().BeFalse();
        dialog.CainActIII.Congratulations.Should().BeTrue();
        dialog.Elzix.Congratulations.Should().BeTrue();

        dialog.Malah.Congratulations.Should().BeFalse();
        dialog.Anya.Congratulations.Should().BeFalse();
        dialog.Natalya.Congratulations.Should().BeTrue();
        dialog.MeshifActIII.Congratulations.Should().BeTrue();
        dialog.Ormus.Congratulations.Should().BeFalse();

        dialog.CainActV.Congratulations.Should().BeFalse();
        dialog.Qualkehk.Congratulations.Should().BeFalse();
        dialog.Nihlathak.Congratulations.Should().BeFalse();
        #endregion Congratulations
    }

    private static void ValidateNightmareDifficulty(NPCDialogDifficulty dialog)
    {
        dialog.Should().NotBeNull();

        #region Introductions
        dialog.WarrivActII.Introduction.Should().BeFalse();
        dialog.Charsi.Introduction.Should().BeTrue();
        dialog.WarrivActI.Introduction.Should().BeTrue();
        dialog.Kashya.Introduction.Should().BeFalse();
        dialog.Akara.Introduction.Should().BeTrue();
        dialog.Gheed.Introduction.Should().BeFalse();

        dialog.Greiz.Introduction.Should().BeFalse();
        dialog.Jerhyn.Introduction.Should().BeTrue();
        dialog.MeshifActII.Introduction.Should().BeTrue();
        dialog.Geglash.Introduction.Should().BeTrue();
        dialog.Lysander.Introduction.Should().BeTrue();
        dialog.Fara.Introduction.Should().BeTrue();
        dialog.Drogan.Introduction.Should().BeFalse();

        dialog.Alkor.Introduction.Should().BeTrue();
        dialog.Hratli.Introduction.Should().BeFalse();
        dialog.Ashera.Introduction.Should().BeTrue();
        dialog.CainActIII.Introduction.Should().BeTrue();
        dialog.Elzix.Introduction.Should().BeTrue();

        dialog.Malah.Introduction.Should().BeTrue();
        dialog.Anya.Introduction.Should().BeFalse();
        dialog.Natalya.Introduction.Should().BeTrue();
        dialog.MeshifActIII.Introduction.Should().BeTrue();
        dialog.Ormus.Introduction.Should().BeTrue();

        dialog.CainActV.Introduction.Should().BeFalse();
        dialog.Qualkehk.Introduction.Should().BeFalse();
        dialog.Nihlathak.Introduction.Should().BeFalse();
        #endregion Introductions

        #region Congratulations
        dialog.WarrivActII.Congratulations.Should().BeFalse();
        dialog.Charsi.Congratulations.Should().BeFalse();
        dialog.WarrivActI.Congratulations.Should().BeTrue();
        dialog.Kashya.Congratulations.Should().BeTrue();
        dialog.Akara.Congratulations.Should().BeFalse();
        dialog.Gheed.Congratulations.Should().BeTrue();

        dialog.Greiz.Congratulations.Should().BeFalse();
        dialog.Jerhyn.Congratulations.Should().BeFalse();
        dialog.MeshifActII.Congratulations.Should().BeFalse();
        dialog.Geglash.Congratulations.Should().BeFalse();
        dialog.Lysander.Congratulations.Should().BeFalse();
        dialog.Fara.Congratulations.Should().BeFalse();
        dialog.Drogan.Congratulations.Should().BeTrue();

        dialog.Alkor.Congratulations.Should().BeTrue();
        dialog.Hratli.Congratulations.Should().BeTrue();
        dialog.Ashera.Congratulations.Should().BeFalse();
        dialog.CainActIII.Congratulations.Should().BeFalse();
        dialog.Elzix.Congratulations.Should().BeTrue();

        dialog.Malah.Congratulations.Should().BeFalse();
        dialog.Anya.Congratulations.Should().BeFalse();
        dialog.Natalya.Congratulations.Should().BeTrue();
        dialog.MeshifActIII.Congratulations.Should().BeTrue();
        dialog.Ormus.Congratulations.Should().BeFalse();

        dialog.CainActV.Congratulations.Should().BeFalse();
        dialog.Qualkehk.Congratulations.Should().BeFalse();
        dialog.Nihlathak.Congratulations.Should().BeFalse();
        #endregion Congratulations
    }

    private static void ValidateHellDifficulty(NPCDialogDifficulty dialog)
    {
        dialog.Should().NotBeNull();

        #region Introductions
        dialog.WarrivActII.Introduction.Should().BeFalse();
        dialog.Charsi.Introduction.Should().BeFalse();
        dialog.WarrivActI.Introduction.Should().BeFalse();
        dialog.Kashya.Introduction.Should().BeFalse();
        dialog.Akara.Introduction.Should().BeFalse();
        dialog.Gheed.Introduction.Should().BeFalse();

        dialog.Greiz.Introduction.Should().BeFalse();
        dialog.Jerhyn.Introduction.Should().BeFalse();
        dialog.MeshifActII.Introduction.Should().BeFalse();
        dialog.Geglash.Introduction.Should().BeFalse();
        dialog.Lysander.Introduction.Should().BeFalse();
        dialog.Fara.Introduction.Should().BeFalse();
        dialog.Drogan.Introduction.Should().BeFalse();

        dialog.Alkor.Introduction.Should().BeFalse();
        dialog.Hratli.Introduction.Should().BeFalse();
        dialog.Ashera.Introduction.Should().BeFalse();
        dialog.CainActIII.Introduction.Should().BeFalse();
        dialog.Elzix.Introduction.Should().BeFalse();

        dialog.Malah.Introduction.Should().BeFalse();
        dialog.Anya.Introduction.Should().BeFalse();
        dialog.Natalya.Introduction.Should().BeFalse();
        dialog.MeshifActIII.Introduction.Should().BeFalse();
        dialog.Ormus.Introduction.Should().BeFalse();

        dialog.CainActV.Introduction.Should().BeFalse();
        dialog.Qualkehk.Introduction.Should().BeFalse();
        dialog.Nihlathak.Introduction.Should().BeFalse();
        #endregion Introductions

        #region Congratulations
        dialog.WarrivActII.Congratulations.Should().BeFalse();
        dialog.Charsi.Congratulations.Should().BeFalse();
        dialog.WarrivActI.Congratulations.Should().BeFalse();
        dialog.Kashya.Congratulations.Should().BeFalse();
        dialog.Akara.Congratulations.Should().BeFalse();
        dialog.Gheed.Congratulations.Should().BeFalse();

        dialog.Greiz.Congratulations.Should().BeFalse();
        dialog.Jerhyn.Congratulations.Should().BeFalse();
        dialog.MeshifActII.Congratulations.Should().BeFalse();
        dialog.Geglash.Congratulations.Should().BeFalse();
        dialog.Lysander.Congratulations.Should().BeFalse();
        dialog.Fara.Congratulations.Should().BeFalse();
        dialog.Drogan.Congratulations.Should().BeFalse();

        dialog.Alkor.Congratulations.Should().BeFalse();
        dialog.Hratli.Congratulations.Should().BeFalse();
        dialog.Ashera.Congratulations.Should().BeFalse();
        dialog.CainActIII.Congratulations.Should().BeFalse();
        dialog.Elzix.Congratulations.Should().BeFalse();

        dialog.Malah.Congratulations.Should().BeFalse();
        dialog.Anya.Congratulations.Should().BeFalse();
        dialog.Natalya.Congratulations.Should().BeFalse();
        dialog.MeshifActIII.Congratulations.Should().BeFalse();
        dialog.Ormus.Congratulations.Should().BeFalse();

        dialog.CainActV.Congratulations.Should().BeFalse();
        dialog.Qualkehk.Congratulations.Should().BeFalse();
        dialog.Nihlathak.Congratulations.Should().BeFalse();
        #endregion Congratulations
    }
}
