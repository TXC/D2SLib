using D2SImporter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    [FileName("Sets.txt")]
    [PostImportRow(typeof(Action<IImporter, Set, Dictionary<string, string>>), typeof(Set), nameof(PostRowImport))]
    public class Set : ID2Data
    {
        [JsonIgnore, ColumnName("index"), Key]
        public string Index { get; set; } = string.Empty;

        [JsonIgnore, ColumnName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore, ColumnName("level")]
        public int Level { get; set; }

        public List<SetItem> SetItems { get; set; } = [];
        public List<ItemProperty> PartialProperties { get; set; } = [];
        public List<ItemProperty> FullProperties { get; set; } = [];

        static void PostRowImport(IImporter importer, Set set, Dictionary<string, string> row)
        {
            SetPartialProperties(importer, set, row);
            SetFullProperties(importer, set, row);

            set.SetItems = importer.SetItems.Where(x => x.Value.Set == set.Index)
                                            .Select(x => x.Value)
                                            .ToList();
        }

        static void SetPartialProperties(IImporter importer, Set set, Dictionary<string, string> row)
        {
            var propList = new List<PropertyInfo>();
            // Add the properties
            for (int i = 2; i <= 5; i++)
            {
                propList.Add(new PropertyInfo(row[$"PCode{i}a"], row[$"PParam{i}a"],
                                                   row[$"PMin{i}a"], row[$"PMax{i}a"]));
                propList.Add(new PropertyInfo(row[$"PCode{i}b"], row[$"PParam{i}b"],
                                                   row[$"PMin{i}b"], row[$"PMax{i}b"]));
            }

            try
            {
                set.PartialProperties = [.. ItemProperty.GetProperties(importer, propList, set.Level)
                                                        .OrderByDescending(ItemProperty.ByDescriptionPriority)
                                                        .OrderBy(x => x.Index)];
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(new Exception($"Could not get properties for set '{set.Name}' in Sets.txt", e));
            }
        }

        static void SetFullProperties(IImporter importer, Set set, Dictionary<string, string> row)
        {
            var propList = new List<PropertyInfo>();
            // Add the properties
            for (int i = 1; i <= 8; i++)
            {
                propList.Add(new PropertyInfo(row[$"FCode{i}"], row[$"FParam{i}"],
                                                   row[$"FMin{i}"], row[$"FMax{i}"]));
            }

            try
            {
                set.FullProperties = [.. ItemProperty.GetProperties(importer, propList, set.Level)
                                                     .OrderByDescending(ItemProperty.ByDescriptionPriority)];
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(new Exception($"Could not get properties for set '{set.Name}' in Sets.txt", e));
            }
        }
    }
}
