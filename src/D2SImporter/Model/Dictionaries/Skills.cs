using D2Shared;
using D2Shared.Enums;
using D2SImporter.Attributes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    [FileName("Skills.txt")]
    public class Skill : ID2Data
    {
        [ColumnName("skill")]
        public string Name { get; set; }

        [JsonIgnore, ColumnName("*Id"), Key]
        public int Id { get; set; }

        [ColumnName("charclass")]
        public CharacterClass CharClass { get; set; }

        [JsonIgnore, ColumnName("skilldesc")]
        public string SkillDesc { get; set; }

        [ColumnName("skpoints")]
        public int? SkillPoints { get; set; }

        [ColumnName("reqlevel")]
        public int RequiredLevel { get; set; }

        [ColumnName("reqskill1")]
        public string RequiredSkill1 { get; set; }

        [ColumnName("reqskill2")]
        public string RequiredSkill2 { get; set; }

        [ColumnName("reqskill3")]
        public string RequiredSkill3 { get; set; }

        [ColumnName("maxlvl")]
        public int? MaxLevel { get; set; }

        [ColumnName("reqstr")]
        public int? RequiredStrength { get; set; }

        [ColumnName("reqdex")]
        public int? RequiredDexterity { get; set; }

        [ColumnName("reqint")]
        public int? RequiredIntelligence { get; set; }

        [ColumnName("reqvit")]
        public int? RequiredVitality { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
