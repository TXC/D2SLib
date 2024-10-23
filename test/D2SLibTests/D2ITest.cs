using D2SLib;
using D2Shared.Enums;
using D2SLib.Model.Save;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace D2SLibTests;

[TestClass]
public class D2ITest
{
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

    [TestMethod]
    public void VerifyCanReadSharedStash115()
    {
        //0x61 == 1.15
        D2I stash = Core.ReadD2I(File.ReadAllBytes(@"Resources/stash/SharedStashSoftCoreV1_0x63.d2i"), 0x61);

        stash.ItemList.Count.Should().Be(8);
        stash.ItemList.Items[0].Code.Should().Be("rng ");
        //Assert.IsTrue(stash.ItemList.Count == 8);
        //Assert.IsTrue(stash.ItemList.Items[0].Code == "rng ");
    }

    [TestMethod]
    public void ShouldReadItem()
    {
        Item item = Core.ReadItem(File.ReadAllBytes(path: @$"Resources/items/tal-rasha-lidless-eye.d2i"), SaveVersion.v11x);
        //item.Name.Should().Be("Complex");
        item.Should().NotBeNull();
    }

    /*
    [TestMethod]
    [DataRow("_LOD_SharedStashSave.sss")]           // PlugY-file
    [DataRow("PrivateStash.d2x")]                   // PlugY-file
    [DataRow("SharedStashSoftCoreV2.d2i")]          //
    [DataRow("SharedStashSoftCoreV2_0x63.d2i")]     //
    public void VerifyCanReadSharedStash115(string filename, SaveVersion version)
    {
        Core.Importer.LoadFromAssembly();

        //0x61 == 1.15
        D2I stash = Core.ReadD2I(File.ReadAllBytes(@"Resources/stash/{filename}"), 0x61);

        stash.ItemList.Count.Should().Be(8);
        stash.ItemList.Items[0].Code.Should().Be("rng ");
        //Assert.IsTrue(stash.ItemList.Count == 8);
        //Assert.IsTrue(stash.ItemList.Items[0].Code == "rng ");
    }
    */
}
