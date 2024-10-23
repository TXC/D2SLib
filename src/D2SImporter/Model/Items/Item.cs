using D2SImporter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using D2Shared;
using D2Shared.Enums;

namespace D2SImporter.Model
{
    public class Item
    {
        [JsonIgnore]
        public static Item CurrentItem { get; set; }

        [ColumnName("index", "Name", "name"), Translatable]
        public string Name { get; set; } = string.Empty;

        [ColumnName("index", "Name", "name"), Key]
        public string Index { get; set; } = string.Empty;

        [ColumnName("enabled")]
        public bool Enabled { get; set; }

        [ColumnName("lvl")]
        public int ItemLevel { get; set; }

        [ColumnName("lvl req")]
        public int RequiredLevel { get; set; }

        [ColumnName("code", "Name", "item")]
        public string Code { get; set; } = string.Empty;

        [JsonIgnore, ComplexImport(typeof(Func<IImporter, Item, Dictionary<string, string>, List<ItemProperty>>), typeof(Item), nameof(ImportProperties))]
        public List<ItemProperty> Properties { get; set; } = [];
        public bool DamageArmorEnhanced { get; set; }

        [ColumnName("code", "Name", "item")]
        public Equipment? Equipment { get; set; }

        public Item()
        {
            CurrentItem = this;
        }

        protected static List<ItemProperty>? ImportProperties(IImporter importer, Item item, Dictionary<string, string> row)
        {
            return [];
        }


        public static void AddDamageArmorString(Item item)
        {
            if (item == null)
            {
                throw new Exception("The unique item does not exist, something is wrong with one of your unique items.");
            }

            if (item.Equipment == null)
            {
                throw new Exception($"Could not find equipment for item '{item.Name}'");
            }

            if (item.Equipment.EquipmentType == EquipmentType.Weapon)
            {
                AddDamageArmorStringToWeapon(item);
            }
            else if (item.Equipment.EquipmentType == EquipmentType.Armor)
            {
                AddDamageArmorStringToArmor(item);
            }

            // Handle max durability
            var dur = item.Properties.FirstOrDefault(x => x.Property.Code == "dur");
            if (dur != null)
            {
                item.Equipment.Durability += dur.Min ?? 0;
                item.Properties.Remove(dur);
            }

            // Handle indestructible items durability
            if (item.Properties.Any(x => x.Property.Code == "indestruct"))
            {
                item.Equipment.Durability = 0;
            }
        }

        static void AddDamageArmorStringToArmor(Item item)
        {
            // Calculate armor
            if (item.Equipment is not Armor armor)
            {
                return;
            }

            int minAc = armor.MaxAc;
            int maxAc = armor.MaxAc;

            foreach (var property in item.Properties.OrderByDescending(x => x.Property.Code))
            {
                switch (property.Property.Code)
                {
                    case "ac%":
                        minAc = (int)Math.Floor(((minAc + 1) * (100f + property.Min ?? 0) / 100f));
                        maxAc = (int)Math.Floor(((maxAc + 1) * (100f + property.Max ?? 0) / 100f));
                        item.DamageArmorEnhanced = true;
                        break;
                    case "ac":
                        minAc += property.Min ?? 0;
                        maxAc += property.Max ?? 0;
                        item.DamageArmorEnhanced = true;
                        break;
                }
            }

            if (minAc == maxAc)
            {
                armor.ArmorString = $"{maxAc}";
            }
            else
            {
                armor.ArmorString = $"{minAc}-{maxAc}";
            }

            // Handle smite damage
            if (armor.MinDamage.HasValue && armor.MinDamage.Value > 0)
            {
                armor.DamageStringPrefix = armor.Type.Equiv1 switch
                {
                    "shie" or "ashd" => "Smite Damage",
                    "boot" => "Kick Damage",
                    _ => "Unhandled Damage Prefix",
                };
                if (armor.MinDamage.Value == armor.MaxDamage.Value)
                {
                    armor.DamageString = $"{armor.MinDamage.Value}";
                }
                else
                {
                    armor.DamageString = $"{armor.MinDamage.Value} to {armor.MaxDamage.Value}";
                }
            }
        }

        static void AddDamageArmorStringToWeapon(Item item)
        {
            if (item.Equipment is not Weapon weapon)
            {
                return;
            }

            foreach (var damageType in weapon.DamageTypes)
            {
                int minDam1 = damageType.MinDamage;
                int minDam2 = damageType.MinDamage;
                int maxDam1 = damageType.MaxDamage;
                int maxDam2 = damageType.MaxDamage;

                foreach (var property in item.Properties.OrderBy(x => x.Property.Code))
                {
                    switch (property.Property.Code)
                    {
                        case "dmg%":
                            minDam1 = (int)(property.Min / 100f * damageType.MinDamage + damageType.MinDamage);
                            minDam2 = (int)(property.Max / 100f * damageType.MinDamage + damageType.MinDamage);

                            maxDam1 = (int)(property.Min / 100f * damageType.MaxDamage + damageType.MaxDamage);
                            maxDam2 = (int)(property.Max / 100f * damageType.MaxDamage + damageType.MaxDamage);

                            item.DamageArmorEnhanced = true;
                            break;
                        case "dmg-norm":
                            minDam1 += property.Min.Value;
                            minDam2 += property.Min.Value;

                            maxDam1 += property.Max.Value;
                            maxDam2 += property.Max.Value;
                            item.DamageArmorEnhanced = true;
                            break;
                        case "dmg-min":
                            minDam1 += property.Min.Value;
                            minDam2 += property.Max.Value;
                            item.DamageArmorEnhanced = true;
                            break;
                        case "dmg-max":
                            maxDam1 += property.Min.Value;
                            maxDam2 += property.Max.Value;
                            item.DamageArmorEnhanced = true;
                            break;
                    }
                }

                if (minDam1 == minDam2)
                {
                    damageType.DamageString = $"{minDam1} to {maxDam1}";
                }
                else
                {
                    damageType.DamageString = $"({minDam1}-{minDam2}) to ({maxDam1}-{maxDam2})";
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
