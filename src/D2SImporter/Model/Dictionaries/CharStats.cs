using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using D2SImporter.Attributes;
using D2Shared;
using D2Shared.Enums;

namespace D2SImporter.Model
{
    [FileName("CharStats.txt")]
    public class CharStat : ID2Data
    {
        /// <summary>
        /// The name of the character class
        /// </summary>
        [JsonIgnore, ColumnName("class")]
        public string Class { get; set; } = null!;

        /// <summary>
        /// Key Index
        /// </summary>
        [JsonIgnore, Key]
        public string? Index { get => Class?.ToLower()[..3]; }

        /// <summary>
        /// Starting amount of the Strength attribute
        /// </summary>
        [JsonIgnore, ColumnName("str")]
        public int? Strength { get; set; }

        /// <summary>
        /// Starting amount of the Dexterity attribute
        /// </summary>
        [JsonIgnore, ColumnName("dex")]
        public int? Dexterity { get; set; }

        /// <summary>
        /// Starting amount of the Energy attribute
        /// </summary>
        [JsonIgnore, ColumnName("int")]
        public int? Energy { get; set; }

        /// <summary>
        /// Starting amount of the Vitality attribute
        /// </summary>
        [JsonIgnore, ColumnName("vit")]
        public int? Vitality { get; set; }

        /// <summary>
        /// Starting amount of Stamina
        /// </summary>
        [JsonIgnore, ColumnName("stamina")]
        public int? Stamina { get; set; }

        /// <summary>
        /// <para>Bonus starting Life value</para>
        /// <para>This value gets added with the <see cref="Vitality"/> field value
        /// to determine the overall starting amount of Life</para>
        /// </summary>
        [JsonIgnore, ColumnName("hpadd")]
        public int? HealthBonus { get; set; }

        /// <summary>
        /// Number of seconds to regain max Mana.
        /// (If this equals 0 then it will default to 300 seconds)
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? ManaRegen { get; set; }

        /// <summary>
        /// Starting amount of Attack Rating
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? ToHitFactor { get; set; }

        /// <summary>
        /// Base Walk movement speed
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? WalkVelocity { get; set; }

        /// <summary>
        /// Base Run movement speed
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? RunVelocity { get; set; }

        /// <summary>
        /// Rate at which Stamina is lost while running
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? RunDrain { get; set; }

        /// <summary>
        /// <para>Amount of Life added for each level gained</para>
        /// <para>Calculated in fourths and is divided by 256</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? LifePerLevel { get; set; }

        /// <summary>
        /// <para>Amount of Stamina added for each level gained</para>
        /// <para>Calculated in fourths and is divided by 256</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? StaminaPerLevel { get; set; }

        /// <summary>
        /// <para>Amount of Mana added for each level gained</para>
        /// <para>Calculated in fourths and is divided by 256</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? ManaPerLevel { get; set; }

        /// <summary>
        /// <para>Amount of Life added for each point in Vitality</para>
        /// <para>Calculated in fourths and is divided by 256</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? LifePerVitality { get; set; }

        /// <summary>
        /// <para>Amount of Stamina added for each point in Vitality</para>
        /// <para>Calculated in fourths and is divided by 256</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? StaminaPerVitality { get; set; }

        /// <summary>
        /// <para>Amount of Mana added for each point in Energy</para>
        /// <para>Calculated in fourths and is divided by 256</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? ManaPerMagic { get; set; }

        /// <summary>
        /// Amount of Attribute stat points earned for each level gained
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? StatPerLevel { get; set; }

        /// <summary>
        /// Amount of Skill points earned for each level gained
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? SkillsPerLevel { get; set; }

        /// <summary>
        /// Baseline radius size of the character's Light Radius
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? LightRadius { get; set; }

        /// <summary>
        /// Baseline percent chance for Blocking
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? BlockFactor { get; set; }

        /// <summary>
        /// <para>Global delay on all Skills after using a
        /// Skill with a Casting Delay</para>
        /// <para>Calculated in Frames, where 25 Frames = 1 Second)</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? MinimumCastingDelay { get; set; }

        /// <summary>
        /// <para>Controls what skill will be added by default to
        /// the character's starting weapon and will be
        /// slotted in the Right Skill selection</para>
        /// <para>Uses the "skill" field from skills.txt</para>
        /// </summary>
        [JsonIgnore, ColumnName]
        public string? StartSkill { get; set; }

        #region Skill
        /// <summary>
        /// <para>Skill that the character starts with and will
        /// always have available</para>
        /// <para>Uses the "skill" field from skills.txt</para>
        /// </summary>
        [JsonIgnore, ColumnName("Skill 1")]
        public string? Skill1 { get; set; }

        /// <inheritdoc cref="Skill1"/>
        [JsonIgnore, ColumnName("Skill 2")]
        public string? Skill2 { get; set; }

        /// <inheritdoc cref="Skill1"/>
        [JsonIgnore, ColumnName("Skill 3")]
        public string? Skill3 { get; set; }

        /// <inheritdoc cref="Skill1"/>
        [JsonIgnore, ColumnName("Skill 4")]
        public string? Skill4 { get; set; }

        /// <inheritdoc cref="Skill1"/>
        [JsonIgnore, ColumnName("Skill 5")]
        public string? Skill5 { get; set; }

        /// <inheritdoc cref="Skill1"/>
        [JsonIgnore, ColumnName("Skill 6")]
        public string? Skill6 { get; set; }

        /// <inheritdoc cref="Skill1"/>
        [JsonIgnore, ColumnName("Skill 7")]
        public string? Skill7 { get; set; }

        /// <inheritdoc cref="Skill1"/>
        [JsonIgnore, ColumnName("Skill 8")]
        public string? Skill8 { get; set; }

        /// <inheritdoc cref="Skill1"/>
        [JsonIgnore, ColumnName("Skill 9")]
        public string? Skill9 { get; set; }

        /// <inheritdoc cref="Skill1"/>
        [JsonIgnore, ColumnName("Skill 10")]
        public string? Skill10 { get; set; }
        #endregion Skill

        /// <summary>
        /// String key for displaying the item modifier bonus
        /// to all skills for the class (Ex: "+1 to Barbarian Skill Levels")
        /// </summary>
        [JsonIgnore, ColumnName]
        public string StrAllSkills { get; set; } = string.Empty;

        /// <summary>
        /// <inheritdoc cref="StrAllSkills"/>
        /// <para>Translated</para>
        /// </summary>
        [JsonIgnore, ColumnName("StrAllSkills"), Translatable]
        public string AllSkills { get; set; } = string.Empty;

        /// <summary>
        /// String key for displaying the item modifier bonus
        /// to all skills for the class's first skill tab (Ex: "+1 to Warcries")
        /// </summary>
        [JsonIgnore, ColumnName]
        public string StrSkillTab1 { get; set; } = string.Empty;

        /// <summary>
        /// <inheritdoc cref="StrSkillTab1"/>
        /// <para>Translated</para>
        /// </summary>
        [JsonIgnore, ColumnName("StrSkillTab1"), Translatable]
        public string SkillTab1 { get; set; } = string.Empty;

        /// <summary>
        /// String key for displaying the item modifier bonus
        /// to all skills for the class's second skill tab (Ex: "+1 to Combat Skills")
        /// </summary>
        [JsonIgnore, ColumnName]
        public string StrSkillTab2 { get; set; } = string.Empty;

        /// <summary>
        /// <inheritdoc cref="StrSkillTab2"/>
        /// <para>Translated</para>
        /// </summary>
        [JsonIgnore, ColumnName("StrSkillTab2"), Translatable]
        public string SkillTab2 { get; set; } = string.Empty;

        /// <summary>
        /// String key for displaying the item modifier bonus
        /// to all skills for the class's third skill tab (Ex: "+1 to Masteries")
        /// </summary>
        [JsonIgnore, ColumnName]
        public string StrSkillTab3 { get; set; } = string.Empty;

        /// <summary>
        /// <inheritdoc cref="StrSkillTab3"/>
        /// <para>Translated</para>
        /// </summary>
        [JsonIgnore, ColumnName("StrSkillTab3"), Translatable]
        public string SkillTab3 { get; set; } = string.Empty;

        /// <summary>
        /// String key for displaying on item modifier exclusive
        /// to the class or for class specific items 
        /// </summary>
        [JsonIgnore, ColumnName]
        public string StrClassOnly { get; set; } = string.Empty;

        /// <summary>
        /// <inheritdoc cref="StrClassOnly"/>
        /// <para>Translated</para>
        /// </summary>
        [JsonIgnore, ColumnName("StrClassOnly"), Translatable]
        public string ClassOnly { get; set; } = string.Empty;

        /// <summary>
        /// This scales the amount of Life that a Healing potion
        /// will restore based on the class
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? HealthPotionPercent { get; set; }

        /// <summary>
        /// This scales the amount of Mana that a Mana potion
        /// will restore based on the class
        /// </summary>
        [JsonIgnore, ColumnName]
        public int? ManaPotionPercent { get; set; }

        /// <summary>
        /// Base weapon class that the character will use by
        /// default when no weapon is equipped
        /// </summary>
        [JsonIgnore, ColumnName]
        public WeaponClass BaseWeaponClass { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore, ComplexImport(typeof(Func<CharStat, IImporter, Dictionary<string, string>, List<CharStatItem>>), typeof(CharStat), nameof(ImportItems))]
        public List<CharStatItem> Items { get; set; } = [];

        /// <summary>
        /// 
        /// </summary>
        //[JsonIgnore]
        //public Dictionary<string, CharStat> Data { get; set; } = [];

        static List<CharStatItem> ImportItems(CharStat instance, IImporter importer, Dictionary<string, string> row)
        {
            List<CharStatItem> Items = [];
            for (int i = 1; i < 11; i++)
            {
                if (row[$"item{i}"] == "0"
                 && row[$"item{i}loc"] == ""
                 && row[$"item{i}count"]  == "0")
                {
                    continue;
                }
                CharStatItem item = new()
                {
                    Code = row[$"item{i}"],
                    Item = importer.GetByCode(row[$"item{i}"]),
                    Location = Utility.ToEnum<EquippedSlot>(row[$"item{i}loc"]),
                    Count = Utility.ToNullableInt(row[$"item{i}count"]),
                };
                if (row.TryGetValue($"item{i}quality", out var val))
                {
                    item.Quality = (ItemQuality)Utility.ToInt(val);
                }
                Items.Add(item);
            }
            return Items;
        }

        public override string ToString()
        {
            return Class;
        }
    }

    public class CharStatItem
    {
        /// <summary>
        /// Item that the character starts with
        /// Uses ID pointer from <see cref="Weapon"/>, <see cref="Armor"/> or <see cref="Misc"/>
        /// </summary>
        [JsonIgnore]
        public Equipment? Item { get; set; }

        /// <summary>
        /// <para>Item that the character starts with</para>
        /// <para>Reference to <see cref="Equipment.Code"/> string in <see cref="Weapon"/>, <see cref="Armor"/> or <see cref="Misc"/></para>
        /// </summary>
        [JsonIgnore]
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Location where the related item will be placed in the character's inventory
        /// </summary>
        [JsonIgnore]
        public EquippedSlot Location { get; set; }
        
        /// <summary>
        /// The amount of the related item that the character starts with
        /// </summary>
        [JsonIgnore]
        public int? Count { get; set; }

        /// <summary>
        /// Controls the quality level of the related item 
        /// </summary>
        [JsonIgnore]
        public ItemQuality Quality { get; set; }

        public override string ToString()
            => Item is not null ? $"{Item.Name} ({Code})" : $"({Code})";
    }
}
