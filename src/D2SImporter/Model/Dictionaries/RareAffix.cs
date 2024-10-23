using D2SImporter.Attributes;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    public class RareAffix : ID2Data
    {
        [ColumnName("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Controls what item types are allowed for this Rare Affix to spawn on
        /// (Uses the ID field from ItemTypes.txt)
        /// </summary>
        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Dictionary<string, string>, List<ItemType>>), typeof(RareAffix), nameof(ImportIncludedItems))]
        public List<ItemType>? Included { get; set; }

        /// <summary>
        /// Controls what item types are excluded for this Rare Affix to spawn on
        /// (Uses the ID field from ItemTypes.txt)
        /// </summary>
        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Dictionary<string, string>, List<ItemType>>), typeof(RareAffix), nameof(ImportExcludedItems))]
        public List<ItemType>? Excluded { get; set; }

        [JsonIgnore]
        public int Index { get; set; }

        static List<ItemType> ImportIncludedItems(IImporter importer, Dictionary<string, string> row)
        {
            List<ItemType> Items = [];
            for (int i = 1; i <= 7; i++)
            {
                if (row.TryGetValue($"itype{i}", out string value)
                    && !string.IsNullOrEmpty(value))
                {
                    Items.Add(importer.ItemTypes[value]);
                }
            }
            return Items;
        }

        static List<ItemType> ImportExcludedItems(IImporter importer, Dictionary<string, string> row)
        {
            List<ItemType> Items = [];
            for (int i = 1; i <= 4; i++)
            {
                if (row.TryGetValue($"etype{i}", out string value)
                    && !string.IsNullOrEmpty(value))
                {
                    Items.Add(importer.ItemTypes[value]);
                }
            }
            return Items;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
