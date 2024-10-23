using D2SImporter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    [FileName("Gems.txt")]
    public class Gem : Item, ID2Data
    {
        public new string Name { get { return Index; } }

        [JsonIgnore, ColumnName("letter")]
        public string Letter { get; set; }

        [JsonIgnore]
        public int NumMods { get; set; }

        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Gem, Dictionary<string, string>, List<ItemProperty>>), typeof(Gem), nameof(ImportWeaponProperties))]
        public List<ItemProperty> WeaponProperties { get; set; }

        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Gem, Dictionary<string, string>, List<ItemProperty>>), typeof(Gem), nameof(ImportHelmProperties))]
        public List<ItemProperty> HelmProperties { get; set; }

        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Gem, Dictionary<string, string>, List<ItemProperty>>), typeof(Gem), nameof(ImportShieldProperties))]
        public List<ItemProperty> ShieldProperties { get; set; }

        static List<ItemProperty>? ImportWeaponProperties(IImporter importer, Gem instance, Dictionary<string, string> row)
            => ImportProperties("weapon", importer, instance, row);
        static List<ItemProperty>? ImportHelmProperties(IImporter importer, Gem instance, Dictionary<string, string> row)
            => ImportProperties("helm", importer, instance, row);
        static List<ItemProperty>? ImportShieldProperties(IImporter importer, Gem instance, Dictionary<string, string> row)
            => ImportProperties("shield", importer, instance, row);

        static List<ItemProperty>? ImportProperties(string prefix, IImporter importer, Gem instance, Dictionary<string, string> row)
        {
            var propList = new List<PropertyInfo>();
            for (int i = 1; i <= 3; i++)
            {
                propList.Add(new PropertyInfo(row[$"{prefix}Mod{i}Code"],
                                                   row[$"{prefix}Mod{i}Param"],
                                                   row[$"{prefix}Mod{i}Min"],
                                                   row[$"{prefix}Mod{i}Max"]));
            }

            try
            {
                return [.. ItemProperty
                        .GetProperties(importer, propList)
                        .OrderByDescending(ItemProperty.ByDescriptionPriority)];
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(new Exception($"Could not get {prefix} properties for gem '{instance.Index}' in Gems.txt", e));
            }
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
