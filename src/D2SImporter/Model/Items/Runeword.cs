using D2SImporter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    [FileName("Runes.txt")]
    [PostImportRow(typeof(Action<IImporter, Runeword>), typeof(Runeword), nameof(PostRowImport))]
    public class Runeword : Item, ID2Data
    {
        [JsonIgnore]
        public new string Name { get; set; } = string.Empty;

        [JsonIgnore]
        [ComplexImport(typeof(Func<Runeword, Dictionary<string, string>, int>), typeof(Runeword), nameof(ImportIndex))]
        public new int Index { get; set; }

        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Dictionary<string, string>, List<Misc>>), typeof(Runeword), nameof(ImportRunes))]
        public List<Misc> Runes { get; set; }

        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Dictionary<string, string>, List<ItemType>>), typeof(Runeword), nameof(ImportTypes))]
        public List<ItemType> Types { get; set; }

        [JsonIgnore]
        public Dictionary<int, Runeword> Data { get; set; } = [];

        static int ImportIndex(Runeword instance, Dictionary<string, string> row)
        {
            if (instance.Name == string.Empty && row.TryGetValue("Name", out string value))
            {
                instance.Name = value;
            }
            if (instance.Name == string.Empty)
            {
                throw new NullReferenceException("Unable to find index");
            }
            int id = int.Parse(instance.Name[8..]);
            if (id > 75)
            {
                id += 75;
            }
            else
            {
                id += 26;
            }
            return id;
        }

        static List<Misc>? ImportRunes(IImporter importer, Dictionary<string, string> row)
        {
            if (string.IsNullOrEmpty(row["Rune1"]))
            {
                return null;
            }

            // Add the runes
            var runeArray = new string[] { row["Rune1"], row["Rune2"], row["Rune3"],
                                                   row["Rune4"], row["Rune5"], row["Rune6"] };
            var runes = new List<Misc>();

            for (int i = 0; i < runeArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(runeArray[i]) && !runeArray[i].StartsWith('*'))
                {
                    runes.Add(importer.Miscs[runeArray[i]]);
                }
            }
            return runes;
        }

        static List<ItemType>? ImportTypes(IImporter importer, Dictionary<string, string> row)
        {
            // Add the types
            var typeArray = new string[] { row["itype1"], row["itype2"], row["itype3"],
                                                   row["itype4"], row["itype5"], row["itype6"] };
            var types = new List<ItemType>();

            for (int i = 0; i < typeArray.Length; i++)
            {
                if (string.IsNullOrEmpty(typeArray[i])
                    || typeArray[i].StartsWith('*'))
                {
                    continue;
                }
                var type = importer.ItemTypes[typeArray[i]];
                types.Add(type);
            }
            return types;
        }

        protected static new List<ItemProperty>? ImportProperties(IImporter importer, Item item, Dictionary<string, string> row)
        {
            if (item is not Runeword instance)
            {
                return [];
            }

            var propList = new List<PropertyInfo>();
            for (int i = 1; i <= 7; i++)
            {
                propList.Add(new PropertyInfo(row[$"T1Code{i}"], row[$"T1Param{i}"], row[$"T1Min{i}"], row[$"T1Max{i}"]));
            }

            var props = new List<ItemProperty>();
            try
            {
                props = [.. ItemProperty.GetProperties(importer, propList)
                                                       .OrderByDescending(ItemProperty.ByDescriptionPriority)];
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(new Exception($"Could not get properties for runeword '{instance.Name}' in Runes.txt", e));
            }

            var shieldCounted = instance.Types.Where(x => x.Equiv1 == "shld" || x.Code == "shld").Any();
            var weaponCounted = instance.Types.Where(x => x.BodyLocation1 == "rarm" || x.Code == "weap").Any();
            var armorCounted = instance.Types.Where(x => x.Equiv1 != "shld"
                                                                 && x.BodyLocation1 != "rarm"
                                                                 && x.Code != "shld"
                                                                 && x.Code != "weap").Any();

            int typeCount = 0;
            typeCount += shieldCounted ? 1 : 0;
            typeCount += weaponCounted ? 1 : 0;
            typeCount += armorCounted ? 1 : 0;

            // Add rune properties
            foreach (var rune in instance.Runes)
            {
                if (!importer.Gems.TryGetValue(rune.Name, out Gem runeGem))
                {
                    ExceptionHandler.LogException(new Exception($"Could not find rune '{rune.Name}' in Runes.txt"));
                }

                //var runeGem = Core.Importer.Gems.Data[rune.Name];
                var wepAdded = false;
                var shieldAdded = false;
                var armorAdded = false;

                foreach (var type in instance.Types)
                {
                    if (type.Equiv1 == "shld" || type.Code == "shld") // Shield
                    {
                        if (!shieldAdded)
                        {
                            var properties = runeGem.ShieldProperties.Select(x => new ItemProperty(x)).ToList();

                            if (typeCount > 1)
                            {
                                properties.ForEach(x => x.Suffix = " (Shield)");
                            }

                            props.AddRange(properties);
                        }
                        shieldAdded = true;
                    }
                    else if (type.BodyLocation1 == "rarm" || type.Code == "weap") // Weapon
                    {
                        if (!wepAdded)
                        {
                            var properties = runeGem.WeaponProperties.Select(x => new ItemProperty(x)).ToList();

                            if (typeCount > 1)
                            {
                                properties.ForEach(x => x.Suffix = " (Weapon)");
                            }

                            props.AddRange(properties);
                        }
                        wepAdded = true;
                    }
                    else // Armor
                    {
                        if (!armorAdded)
                        {
                            var properties = runeGem.HelmProperties.Select(x => new ItemProperty(x)).ToList();

                            if (typeCount > 1)
                            {
                                properties.ForEach(x => x.Suffix = " (Armor)");
                            }

                            props.AddRange(properties);
                        }
                        armorAdded = true;
                    }
                }
            }

            return props;
        }

        static void PostRowImport(IImporter importer, Runeword item)
        {
            if (item.Properties.Count > 0)
            {
                ItemProperty.CleanupDublicates(importer, item.Properties);
            }
        }
    }
}
