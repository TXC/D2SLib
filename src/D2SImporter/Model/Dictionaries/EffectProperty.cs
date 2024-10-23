using D2Shared;
using D2SImporter.Attributes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    [FileName("Properties.txt")]
    public class EffectProperty : ID2Data
    {
        [JsonIgnore, ColumnName("code"), Key]
        public string Code { get; set; }

        [JsonIgnore, ColumnName("stat1")]
        public string Stat1 { get; set; } = string.Empty;

        [JsonIgnore, ColumnName("stat2")]
        public string Stat2 { get; set; } = string.Empty;

        [JsonIgnore, ColumnName("stat3")]
        public string Stat3 { get; set; } = string.Empty;

        [JsonIgnore, ColumnName("stat4")]
        public string Stat4 { get; set; } = string.Empty;

        [JsonIgnore, ColumnName("stat5")]
        public string Stat5 { get; set; } = string.Empty;

        [JsonIgnore, ColumnName("stat6")]
        public string Stat6 { get; set; } = string.Empty;

        [JsonIgnore, ColumnName("stat7")]
        public string Stat7 { get; set; } = string.Empty;

        [JsonIgnore, ColumnName("func1")]
        public int? Function1 { get; set; }

        [JsonIgnore, ColumnName("func2")]
        public int? Function2 { get; set; }

        [JsonIgnore, ColumnName("func3")]
        public int? Function3 { get; set; }

        [JsonIgnore, ColumnName("func4")]
        public int? Function4 { get; set; }

        [JsonIgnore, ColumnName("func5")]
        public int? Function5 { get; set; }

        [JsonIgnore, ColumnName("func6")]
        public int? Function6 { get; set; }

        [JsonIgnore, ColumnName("func7")]
        public int? Function7 { get; set; }

        public override string ToString()
        {
            return Code;
        }
    }
}
