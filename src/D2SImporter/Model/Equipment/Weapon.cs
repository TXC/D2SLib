using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using D2SImporter.Attributes;
using D2Shared;
using D2Shared.Enums;

namespace D2SImporter.Model
{
    [FileName("Weapons.txt")]
    public class Weapon : Equipment, ID2Data
    {
        [JsonIgnore, ColumnName("rangeadder")]
        public int? RangeAdder { get; set; }

        [JsonIgnore, ColumnName("wclass")]
        public string OneHandClass{ get; set; }

        [JsonIgnore, ColumnName("2handedwclass")]
        public string TwoHandClass { get; set; }

        [JsonIgnore, ColumnName("hit class")]
        public string HitClass { get; set; }

        [JsonIgnore, ComplexImport(typeof(Func<Weapon, Dictionary<string, string>, List<DamageType>>), typeof(Weapon), nameof(ImportDamageTypes))]
        public List<DamageType> DamageTypes { get; set; }

        public Weapon() : base()
        {
            EquipmentType = EquipmentType.Weapon;
        }

        /// <summary>
        /// <list type="bullet">
        /// <item>
        ///     <term>1or2handed</term>
        ///     <description>
        ///     <para>If set, then the item will be treated as a one-handed and
        ///     two-handed weapon by the Barbarian class.</para>
        ///     <para>If not set, then the Barbarian can only use this weapon
        ///     as either one-handed or two-handed, but not both.</para>
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>2handed</term>
        ///     <description>
        ///     <para>If set, then the item will be treated as two-handed weapon.</para>
        ///     <para>If not set, then the item will be treated as one-handed weapon.</para>
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>2handmindam</term>
        ///     <description>
        ///     <para>The minimum physical damage provided by the weapon if the item is two-handed.</para>
        ///     <para>This relies on the "2handed" field being enabled.</para>
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>2handmaxdam</term>
        ///     <description>
        ///     <para>The maximum physical damage provided by the weapon if the item is two - handed.</para>
        ///     <para>This relies on the "2handed" field being enabled.</para>
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>hit class</term>
        ///     <description>
        ///     Defines the hit class of the weapon which is used to know what
        ///     SFX to use when the weapon hits an enemy
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>minmisdam</term>
        ///     <description>
        ///     The maximum physical damage provided by the item if it is a throwing weapon
        ///     </description>
        /// </item>
        /// <item>
        ///     <term>maxmisdam</term>
        ///     <description>
        ///     The maximum physical damage provided by the item if it is a throwing weapon
        ///     </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        static List<DamageType> ImportDamageTypes(Weapon weapon, Dictionary<string, string> row)
        {
            var damageTypes = new List<DamageType>();

            var isOneOrTwoHanded = row["1or2handed"] == "1";
            var isTwoHanded = row["2handed"] == "1";
            var isThrown = !string.IsNullOrEmpty(row["minmisdam"]);
            var name = row["name"];

            if (!isTwoHanded)
            {
                try
                {
                    damageTypes.Add(new DamageType
                    {
                        Type = DamageTypeEnum.Normal,
                        MinDamage = int.Parse(row["mindam"]),
                        MaxDamage = int.Parse(row["maxdam"])
                    });
                }
                catch (Exception)
                {
                    ExceptionHandler.LogException(new Exception($"Could not get min or max damage for weapon: '{name}' in Weapons.txt"));
                }
            }
            else if (isOneOrTwoHanded)
            {
                try
                {
                    damageTypes.Add(new DamageType
                    {
                        Type = DamageTypeEnum.OneHanded,
                        MinDamage = int.Parse(row["mindam"]),
                        MaxDamage = int.Parse(row["maxdam"])
                    });
                }
                catch (Exception)
                {
                    ExceptionHandler.LogException(new Exception($"Could not get min or max one handed damage for weapon: '{name}' in Weapons.txt"));
                }
            }

            if (isTwoHanded)
            {
                try
                {
                    damageTypes.Add(new DamageType
                    {
                        Type = DamageTypeEnum.TwoHanded,
                        MinDamage = int.Parse(row["2handmindam"]),
                        MaxDamage = int.Parse(row["2handmaxdam"])
                    });
                }
                catch (Exception)
                {
                    ExceptionHandler.LogException(new Exception($"Could not get min or max two handed damage for weapon: '{name}' in Weapons.txt"));
                }
            }

            if (isThrown)
            {
                try
                {
                    damageTypes.Add(new DamageType
                    {
                        Type = DamageTypeEnum.Thrown,
                        MinDamage = int.Parse(row["minmisdam"]),
                        MaxDamage = int.Parse(row["maxmisdam"])
                    });
                }
                catch (Exception)
                {
                    ExceptionHandler.LogException(new Exception($"Could not get min or max thrown damage for weapon: '{name}' in Weapons.txt"));
                }
            }
            return damageTypes;
        }

        public new object Clone()
        {
            var dmgTypes = new List<DamageType>();
            this.DamageTypes.ForEach(x => dmgTypes.Add((DamageType)x.Clone()));

            Weapon eq = base.Clone() as Weapon;
            eq.EquipmentType = this.EquipmentType;

            eq.RangeAdder = this.RangeAdder;
            eq.OneHandClass = this.OneHandClass;
            eq.TwoHandClass = this.TwoHandClass;
            eq.HitClass = this.HitClass;
            eq.DamageTypes = dmgTypes;

            return eq;
        }
    }
}
