using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    public class ItemProperty
    {
        [JsonIgnore]
        public static ItemProperty CurrentItemProperty { get; set; }

        [JsonIgnore]
        public EffectProperty Property { get; set; }

        [JsonIgnore]
        public string Parameter { get; set; }

        [JsonIgnore]
        public int? Min { get; set; }

        [JsonIgnore]
        public int? Max { get; set; }

        [JsonIgnore]
        public ItemStatCost ItemStatCost { get; set; }

        private string _propertyString;
        public string PropertyString { get => _propertyString + Suffix; set { _propertyString = value; } }
        public int Index { get; set; }

        [JsonIgnore]
        public int ItemLevel { get; set; }

        [JsonIgnore]
        public string Suffix { get; set; }

        [JsonIgnore]
        private static List<string> _ignoredProperties = new List<string> { "state", "bloody" };

        [JsonIgnore]
        public string CompareKey => ItemStatCost.Stat + Parameter;

        public ItemProperty(IImporter importer, string property, string parameter,
                            int? min, int? max, int index, int itemLevel = 0, string suffix = "")
        {
            CurrentItemProperty = this;

            Parameter = parameter;
            Min = min;
            Max = max;
            Index = index;
            ItemLevel = itemLevel;
            Suffix = suffix;

            if (!importer.EffectProperties.ContainsKey(property.ToLower()))
            {
                throw ItemPropertyException.Create($"Could not find property '{property.ToLower()}' parameter '{parameter}' min '{min}' max '{max}' index '{index}' itemlvl '{itemLevel}' in Properties.txt");
            }

            Property = importer.EffectProperties[property.ToLower()];

            var stat = Property.Stat1;


            if (string.IsNullOrEmpty(stat))
            {
                // Fix wrongly spelled and hardcoded stats
                switch (Property.Code)
                {
                    case "str":
                        stat = "strength";
                        break;
                    case "dmg-min":
                        stat = "mindamage";
                        break;
                    case "dmg-max":
                        stat = "maxdamage";
                        break;
                    case "indestruct":
                        stat = "item_indesctructible";
                        break;
                    case "poisonlength":
                        Min = null;
                        Max = null;
                        break;
                    default:
                        stat = Property.Code;
                        break;
                }
            }

            switch (stat)
            {
                case "item_addskill_tab": // Fetch skill tab from .lst files
                    break;
            }

            // Fix all class skills displaying as amazon
            if (stat == "item_addclassskills")
            {
                Parameter = Property.Code;
            }

            // Fix manual set
            switch (Property.Code)
            {
                case "res-all":
                    stat = Property.Code;
                    break;
            }

            if (importer.ItemStatCosts.TryGetValue(stat, out ItemStatCost isc))
            {
                ItemStatCost = isc;
            }
            else
            {
                throw ItemPropertyException.Create($"Could not find stat '{stat}' in ItemStatCost.txt");
            }


            try
            {
                //ItemStatCost = Core.Importer.ItemStatCosts[stat];
                PropertyString = ItemStatCost.PropertyString(importer, Min, Max, Parameter, ItemLevel);
            }
            catch (Exception e)
            {
                if (e as ItemStatCostException == null)
                {
                    throw ItemPropertyException.Create($"Could not generate properties for property '{property}' with parameter '{parameter}' min '{min}' max '{max}' index '{index}' itemlvl '{itemLevel}'", e);
                }

                throw e;
            }
        }

        public ItemProperty(ItemProperty itemProperty)
        {
            CurrentItemProperty = this;
            Property = itemProperty.Property;
            Parameter = itemProperty.Parameter;
            Min = itemProperty.Min;
            Max = itemProperty.Max;
            ItemStatCost = itemProperty.ItemStatCost;
            PropertyString = itemProperty.PropertyString;
            Index = itemProperty.Index;
        }

        public static List<ItemProperty> GetProperties(IImporter importer, List<PropertyInfo> properties, int itemLevel = 0)
        {
            var result = new List<ItemProperty>();

            foreach (var property in properties)
            {
                if (!string.IsNullOrEmpty(property.Property) && !property.Property.StartsWith("*"))
                {
                    var prop = new ItemProperty(importer, property.Property, property.Parameter, property.Min, property.Max, properties.IndexOf(property), itemLevel);
                    if (!_ignoredProperties.Contains(prop.Property.Code.ToLower())) // Don't add hidden stats
                    {
                        result.Add(prop);
                    }
                }
            }

            // Cleanup elemental damage as they are added as 2-3 different parameters
            var minDamage = result.Where(x => x.Property.Stat1.Contains("mindam"));
            var maxDamage = result.Where(x => x.Property.Stat1.Contains("maxdam"));
            var lenDamage = result.Where(x => x.Property.Stat1.Contains("length"));

            if (minDamage.Count() > 0 && maxDamage.Count() > 0)
            {
                var props = new List<ItemProperty>();
                var toRemove = new List<ItemProperty>();
                var toAdd = new List<ItemProperty>();

                foreach (var minDam in minDamage)
                {
                    foreach (var maxDam in maxDamage)
                    {
                        var minDamProperty = minDam.Property.Stat1.Replace("mindam", "");
                        var maxDamProperty = maxDam.Property.Stat1.Replace("maxdam", "");

                        if (minDamProperty == maxDamProperty)
                        {
                            var damagePropertyName = minDamProperty;
                            if (damagePropertyName == "light")
                            {
                                damagePropertyName = "lightning";
                            }

                            damagePropertyName = damagePropertyName.First().ToString().ToUpper() + damagePropertyName.Substring(1);

                            var newProp = new ItemProperty(importer, "eledam",
                                                                      damagePropertyName, minDam.Min,
                                                                      maxDam.Max, minDam.Index);

                            toRemove.Add(minDam);
                            toRemove.Add(maxDam);

                            foreach (var lenDam in lenDamage)
                            {
                                var lenDamProperty = lenDam.Property.Stat1.Replace("length", "");
                                if (minDamProperty == lenDamProperty)
                                {
                                    toRemove.Add(lenDam);
                                }
                            }

                            toAdd.Add(newProp);
                        }
                    }
                }

                toRemove.ForEach(x => result.Remove(x));
                result.AddRange(toAdd);
            }

            // Sometimes there is cold length with min/max damage
            lenDamage = result.Where(x => x.Property.Stat1.Contains("length"));
            if (lenDamage.Count() > 0)
            {
                result.RemoveAll(x => lenDamage.Any(y => y == x && y.Property.Code == "cold-len"));
            }

            // Min damage sometimes contain both elements, weird.
            if (minDamage.Count() > 0 && maxDamage.Count() == 0)
            {
                foreach (var minDam in minDamage)
                {
                    if (minDam.Min != minDam.Max)
                    {
                        minDam.PropertyString = minDam.PropertyString.Replace("+", "Adds ").Replace("Minimum ", "");
                    }
                }
            }

            return result;
        }

        public static void CleanupDublicates(IImporter importer, List<ItemProperty> properties)
        {
            var dupes = properties
                .GroupBy(x => x.CompareKey)
                .Where(x => x.Skip(1).Any())
                .ToList();

            if (dupes.Count > 0)
            {
                foreach (var group in dupes)
                {
                    int? min = 0;
                    int? max = 0;

                    foreach (var prop in group)
                    {
                        min += prop.Min;
                        max += prop.Max;
                    }
                    var newProp = new ItemProperty(importer, group.First().Property.Code, group.First().Parameter, min, max, group.First().Index, group.First().ItemLevel, group.First().Suffix);
                    properties.RemoveAll(x => x.ItemStatCost.Stat == newProp.ItemStatCost.Stat);
                    properties.Add(newProp);
                }
            }
            properties = properties.OrderByDescending(ByDescriptionPriority).ToList();
        }

        public static int? ByDescriptionPriority(ItemProperty property)
            => property.ItemStatCost == null ? 0 : property.ItemStatCost.DescriptionPriority;

        public override string ToString()
        {
            return Property.ToString();
        }
    }
}
