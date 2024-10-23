using D2SImporter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    [FileName("SetItems.txt")]
    [PostImportRow(typeof(Action<SetItem>), typeof(SetItem), nameof(PostRowImport))]
    [PostImport(typeof(Action<Dictionary<string, SetItem>>), typeof(SetItem), nameof(SetPartialPropertyString))]
    public class SetItem : Item, ID2Data
    {
        public string Type { get; set; }

        [JsonIgnore, ColumnName("set")]
        public string Set { get; set; }

        [JsonIgnore, ColumnName("add func")]
        public int AddFunc { get; set; }

        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Item, Dictionary<string, string>, List<ItemProperty>>), typeof(SetItem), nameof(ImportSetProperties))]
        public List<ItemProperty> SetProperties { get; set; }

        public List<string> SetPropertiesString { get; set; }

        protected static new List<ItemProperty>? ImportProperties(IImporter importer,
                                                                  Item item,
                                                                  Dictionary<string, string> row)
        {
            if (item is not SetItem instance)
            {
                return [];
            }

            var propList = new List<PropertyInfo>();
            for (int i = 1; i <= 9; i++)
            {
                propList.Add(new PropertyInfo(row[$"prop{i}"], row[$"par{i}"],
                    row[$"min{i}"], row[$"max{i}"]));
            }

            try
            {
                return [.. ItemProperty.GetProperties(importer, propList, instance.ItemLevel)
                                                      .OrderByDescending(ItemProperty.ByDescriptionPriority)];
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(new Exception($"Could not get set properties for item '{instance.Name}' in SetItems.txt", e));
            }
            return null;
        }

        static List<ItemProperty>? ImportSetProperties(IImporter importer,
                                                       Item item,
                                                       Dictionary<string, string> row)
        {
            if (item is not SetItem instance)
            {
                return [];
            }

            var propList = new List<PropertyInfo>();
            for (int i = 1; i <= 5; i++)
            {
                propList.Add(new PropertyInfo(row[$"aprop{i}a"], row[$"apar{i}a"],
                                                   row[$"amin{i}a"], row[$"amax{i}a"]));
                propList.Add(new PropertyInfo(row[$"aprop{i}b"], row[$"apar{i}b"],
                                                   row[$"amin{i}b"], row[$"amax{i}b"]));
            }

            try
            {
                return [.. ItemProperty.GetProperties(importer, propList, item.ItemLevel)
                                               .OrderByDescending(ItemProperty.ByDescriptionPriority)];
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(new Exception($"Could not get set properties for item '{item.Name}' in SetItems.txt", e));
            }
            return null;
        }

        static void PostRowImport(SetItem item) => AddDamageArmorString(item);

        static void SetPartialPropertyString(Dictionary<string, SetItem> Items)
        {
            foreach (var item in Items)
            {
                item.Value.SetPropertiesString = [];

                foreach (var prop in item.Value.SetProperties)
                {
                    switch (item.Value.AddFunc)
                    {
                        case 0:
                            item.Value.Properties.Add(prop);
                            break;
                        case 1:
                            var setItems = Items.Select(x => x.Value)
                                                  .Where(x => x.Set == item.Value.Set &&
                                                                x.Name != item.Value.Name)
                                                  .ToList();
                            var index = (int)Math.Floor(prop.Index / 2f);

                            item.Value.SetPropertiesString.Add($"{prop.PropertyString} ({setItems[index].Name})");
                            break;
                        case 2:
                            item.Value.SetPropertiesString.Add($"{prop.PropertyString} ({Math.Floor(prop.Index / 2f) + 2} Items)");
                            break;
                    }
                }
            }
        }
    }
}
