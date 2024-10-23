using D2SImporter.Attributes;
using D2Shared.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    [FileName("AutoMagic.txt")]
    public class MagicAffix : ID2Data
    {
        /// <summary>
        /// Defines the item affix name
        /// </summary>
        [ColumnName]
        public string Name { get; set; }

        /// <summary>
        /// Defines which game version to create this item
        /// (0 = Classic mode | 100 = Expansion mode)
        /// </summary>
        [JsonIgnore, ColumnName("version")]
        public int? Version { get; set; }

        /// <summary>
        /// <para>If set, then this item can be randomly spawned.</para>
        /// <para>If not set, then this item will never randomly spawn.</para>
        /// </summary>
        [JsonIgnore, ColumnName("spawnable")]
        public bool Spawnable { get; set; }

        /// <summary>
        /// <para>If set, then this item can be randomly spawned.</para>
        /// <para>If not set, then this item will never randomly spawn.</para>
        /// </summary>
        [JsonIgnore, ColumnName("rare")]
        public bool Rare { get; set; }

        /// <summary>
        /// The minimum item level required for this item affix to spawn on the item.
        /// If the item level is below this value, then the item affix will not spawn on the item.
        /// </summary>
        [JsonIgnore, ColumnName("level")]
        public int? ItemLevel { get; set; }

        /// <summary>
        /// The maximum item level required for this item affix to spawn on the item.
        /// If the item level is above this value, then the item affix will not spawn on the item.
        /// </summary>
        [JsonIgnore, ColumnName("maxlevel")]
        public int? MaxLevel { get; set; }

        /// <summary>
        /// The minimum character level required to equip an item that has this item affix
        /// </summary>
        [JsonIgnore, ColumnName("levelreq")]
        public int? RequiredLevel { get; set; }

        /// <summary>
        /// Controls if this item affix should only be used for class specific items.
        /// This relies on the specified value on <see cref="ItemType.Class"/>, for the specific item.
        /// </summary>
        [JsonIgnore, ColumnName("classspecific")]
        public CharacterClass? ClassSpecific { get; set; }

        /// <summary>
        /// Controls which character class is required for the class specific level requirement "classlevelreq" field
        /// </summary>
        [JsonIgnore, ColumnName("class")]
        public CharacterClass? Class { get; set; }

        /// <summary>
        /// The maximum item level required for this item affix to spawn on the item.
        /// If the item level is above this value, then the item affix will not spawn on the item.
        /// </summary>
        [JsonIgnore, ColumnName("classlevelreq")]
        public int? ClassLevelRequirement { get; set; }

        /// <summary>
        /// The maximum item level required for this item affix to spawn on the item.
        /// If the item level is above this value, then the item affix will not spawn on the item.
        /// </summary>
        [JsonIgnore, ColumnName("frequency")]
        public int Frequency{ get; set; }

        /// <summary>
        /// The maximum item level required for this item affix to spawn on the item.
        /// If the item level is above this value, then the item affix will not spawn on the item.
        /// </summary>
        [JsonIgnore, ColumnName("group")]
        public int Group { get; set; }

        /// <summary>
        /// Controls the item properties for the item affix (Uses the “code” field from Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod1Code")]
        public int Mod1Code { get; set; }

        /// <summary>
        /// The “parameter” value associated with the listed property (mod). Usage depends on the property function (See the “func” field on Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod1Param")]
        public int Mod1Param { get; set; }

        /// <summary>
        /// The “min” value to assign to the listed property (mod). Usage depends on the property function (See the “func” field on Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod1Min")]
        public int Mod1Min { get; set; }

        /// <summary>
        /// The “max” value to assign to the listed property (mod). Usage depends on the property function (See the “func” field on Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod1Max")]
        public int Mod1Max { get; set; }

        /// <summary>
        /// Controls the item properties for the item affix
        /// (Uses <see cref="EffectProperty.Code"/>)
        /// </summary>
        [JsonIgnore, ColumnName("Mod2Code")]
        public int Mod2Code { get; set; }

        /// <summary>
        /// The “parameter” value associated with the listed property (mod).
        /// Usage depends on the property function (See the <see cref="EffectProperty.Function"/>)
        /// </summary>
        [JsonIgnore, ColumnName("Mod2Param")]
        public int Mod2Param { get; set; }

        /// <summary>
        /// The “min” value to assign to the listed property (mod). Usage depends on the property function (See the “func” field on Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod2Min")]
        public int Mod2Min { get; set; }

        /// <summary>
        /// The “max” value to assign to the listed property (mod). Usage depends on the property function (See the “func” field on Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod2Max")]
        public int Mod2Max { get; set; }

        /// <summary>
        /// Controls the item properties for the item affix (Uses the “code” field from Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod3Code")]
        public int? Mod3Code { get; set; }

        /// <summary>
        /// The “parameter” value associated with the listed property (mod). Usage depends on the property function (See the “func” field on Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod3Param")]
        public int? Mod3Param { get; set; }

        /// <summary>
        /// The “min” value to assign to the listed property (mod). Usage depends on the property function (See the “func” field on Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod3Min")]
        public int? Mod3Min { get; set; }

        /// <summary>
        /// The “max” value to assign to the listed property (mod). Usage depends on the property function (See the “func” field on Properties.txt)
        /// </summary>
        [JsonIgnore, ColumnName("Mod3Max")]
        public int? Mod3Max { get; set; }

        /// <summary>
        /// Controls the color change of the item after spawning with this item affix.
        /// If empty, then the item affix will not change the item’s color.
        /// (Uses Color Codes from the reference file colors.txt)
        /// </summary>
        [JsonIgnore, ColumnName("transformcolor")]
        public Color? TransformColor { get; set; }

        /// <summary>
        /// The maximum item level required for this item affix to spawn on the item.
        /// If the item level is above this value, then the item affix will not spawn on the item.
        /// </summary>
        [JsonIgnore, ColumnName("multiply")]
        public int? Multiply { get; set; }

        /// <summary>
        /// Flat integer modification to the item’s buy and sell costs, based on the item affix
        /// </summary>
        [JsonIgnore, ColumnName("add")]
        public int? Add { get; set; }

        [JsonIgnore]
        public int Index { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
