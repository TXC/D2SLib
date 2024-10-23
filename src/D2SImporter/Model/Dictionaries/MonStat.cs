using D2SImporter.Attributes;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    [FileName("MonStats.txt")]
    public class MonStat : ID2Data
    {

        [JsonIgnore, ColumnName]
        public string Id { get; set; }

        [JsonIgnore, ColumnName("*hcIdx"), Key]
        public string Hcldx { get; set; }

        [JsonIgnore, ColumnName]
        public string NameStr { get; set; }

        public override string ToString()
        {
            return NameStr;
        }
    }
}
