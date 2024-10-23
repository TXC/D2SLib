using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using D2SImporter.Attributes;
using D2Shared;
using D2Shared;
using D2Shared.Enums;

namespace D2SImporter.Model
{
    [FileName("Armor.txt")]
    public class Armor : Equipment, ID2Data
    {
        /// <summary>
        /// Controls the block percent chance that the item provides
        /// (out of 100, but caps at 75).
        /// </summary>
        [JsonIgnore, ColumnName("block")]
        public int? Block { get; set; }

        /// <summary>
        /// Controls the character’s graphics and animations for the
        /// Right Arm component when wearing the armor, where the value
        /// 0 = Light or "lit",
        /// 1 = Medium or "med",
        /// and 2 = Heavy or "hvy" 
        /// </summary>
        [JsonIgnore, ColumnName("rArm")]
        public int? RightArm { get; set; }

        /// <summary>
        /// Controls the character’s graphics and animations for the
        /// Left Arm component when wearing the armor, where the value
        /// 0 = Light or "lit",
        /// 1 = Medium or "med",
        /// and 2 = Heavy or "hvy" 
        /// </summary>
        [JsonIgnore, ColumnName("lArm")]
        public int? LeftArm { get; set; }

        /// <summary>
        /// Controls the character’s graphics and animations for the
        /// Torso component when wearing the armor, where the value
        /// 0 = Light or "lit",
        /// 1 = Medium or "med",
        /// and 2 = Heavy or "hvy" 
        /// </summary>
        [JsonIgnore, ColumnName("Torso")]
        public int? Torso { get; set; }

        /// <summary>
        /// Controls the character’s graphics and animations for the
        /// Legs component when wearing the armor, where the value
        /// 0 = Light or "lit",
        /// 1 = Medium or "med",
        /// and 2 = Heavy or "hvy" 
        /// </summary>
        [JsonIgnore, ColumnName("Legs")]
        public int? Legs { get; set; }

        /// <summary>
        /// Controls the character’s graphics and animations for the
        /// Right Shoulder Pad component when wearing the armor, where the value
        /// 0 = Light or "lit",
        /// 1 = Medium or "med",
        /// and 2 = Heavy or "hvy" 
        /// </summary>
        [JsonIgnore, ColumnName("rSPad")]
        public int? RightShoulderPad { get; set; }

        /// <summary>
        /// Controls the character’s graphics and animations for the
        /// Left Shoulder Pad component when wearing the armor, where the value
        /// 0 = Light or "lit",
        /// 1 = Medium or "med",
        /// and 2 = Heavy or "hvy" 
        /// </summary>
        [JsonIgnore, ColumnName("lSPad")]
        public int? LeftShoulderPad { get; set; }

        /// <summary>
        /// The minimum amount of Defense that an armor item type can have
        /// </summary>
        [JsonIgnore, ColumnName("minac")]
        public int MinAc { get; set; }

        /// <summary>
        /// The maximum amount of Defense that an armor item type can have
        /// </summary>
        [JsonIgnore, ColumnName("maxac")]
        public int MaxAc { get; set; }

        [JsonIgnore]
        public Dictionary<string, Armor> Data { get; set; } = [];
        public string DamageString { get; set; }
        public string DamageStringPrefix { get; set; }
        public string ArmorString { get; set; }

        public Armor() : base()
        {
            EquipmentType = EquipmentType.Armor;
        }

        public new object Clone()
        {
            Armor eq = base.Clone() as Armor;
            eq.EquipmentType = this.EquipmentType;
            eq.MinAc = this.MinAc;
            eq.MaxAc = this.MaxAc;
            eq.Type = this.Type;
            eq.Block = this.Block;
            eq.RightArm = this.RightArm;
            eq.LeftArm = this.LeftArm;
            eq.Torso = this.Torso;
            eq.Legs = this.Legs;
            eq.RightShoulderPad = this.RightShoulderPad;
            eq.LeftShoulderPad = this.LeftShoulderPad;

            return eq;
        }
    }
}
