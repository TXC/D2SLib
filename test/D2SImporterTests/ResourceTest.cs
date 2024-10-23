using D2SImporter;
using FluentAssertions;

namespace D2SLibTests;

[TestClass]
public class ResourceTest
{
    FromAssemblyImporter Importer = new();

    [ClassInitialize]
    public static void InitialSetUp(TestContext ctx)
    {
        //Core.Importer.LoadData();
    }

    [TestMethod]
    [DataRow("96")]
    [DataRow("99")]
    public void VerifyCanReadDataFiles(string version)
    {
        ExceptionHandler.ContinueOnException = true;
        Importer.LoadData(version);

        Importer.Table.Count.Should().BeGreaterThan(0);
        //Importer.Table.Data.Should().HaveCountGreaterThan(0);
        Importer.MagicPrefixes.Should().HaveCountGreaterThan(0);
        Importer.MagicSuffixes.Should().HaveCountGreaterThan(0);
        Importer.ItemStatCosts.Should().HaveCountGreaterThan(0);
        Importer.EffectProperties.Should().HaveCountGreaterThan(0);
        Importer.ItemTypes.Should().HaveCountGreaterThan(0);
        Importer.Armors.Should().HaveCountGreaterThan(0);
        Importer.Weapons.Should().HaveCountGreaterThan(0);
        Importer.Skills.Should().HaveCountGreaterThan(0);
        Importer.CharStats.Should().HaveCountGreaterThan(0);
        Importer.MonStats.Should().HaveCountGreaterThan(0);
        Importer.Miscs.Should().HaveCountGreaterThan(0);
        Importer.Gems.Should().HaveCountGreaterThan(0);
        Importer.SetItems.Should().HaveCountGreaterThan(0);
    }

    [TestMethod]
    [DataRow("96")]
    [DataRow("99")]
    public void VerifyCanReadModelFiles(string version)
    {
        VerifyCanReadDataFiles(version);

        Importer.ImportModel(version);

        Importer.Uniques.Should().HaveCountGreaterThan(0);
        Importer.Runewords.Should().HaveCountGreaterThan(0);
        Importer.CubeRecipes.Should().HaveCountGreaterThan(0);
        Importer.Sets.Should().HaveCountGreaterThan(0);
    }
}
