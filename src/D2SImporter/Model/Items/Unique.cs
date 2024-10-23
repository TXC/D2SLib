using D2SImporter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace D2SImporter.Model
{
    [FileName("UniqueItems.txt")]
    [PostImportRow(typeof(Action<Unique>), typeof(Unique), nameof(PostRowImport))]
    public class Unique : Item, ID2Data
    {
        public string Type { get; set; }

        protected new static List<ItemProperty>? ImportProperties(IImporter importer, Item item, Dictionary<string, string> row)
        {
            if (item is not SetItem instance)
            {
                return [];
            }

            var propList = new List<PropertyInfo>();
            // Add the properties
            for (int i = 1; i <= 12; i++)
            {
                propList.Add(new PropertyInfo(row[$"prop{i}"], row[$"par{i}"], row[$"min{i}"], row[$"max{i}"]));
            }

            try
            {
                List<ItemProperty> properties = [.. ItemProperty.GetProperties(importer, propList, instance.ItemLevel)
                                                            .OrderByDescending(ItemProperty.ByDescriptionPriority)];

                return properties;
            }
            catch (Exception e)
            {
                ExceptionHandler.LogException(new Exception($"Could not get properties for unique '{instance.Name}' in UniqueItems.txt", e));
            }
            return null;
        }

        static void PostRowImport(Unique item) => AddDamageArmorString(item);
    }
}
