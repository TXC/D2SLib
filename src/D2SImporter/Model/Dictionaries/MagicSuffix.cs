using D2SImporter.Attributes;

namespace D2SImporter.Model
{
    [FileName("MagicSuffix.txt")]
    public class MagicSuffix : MagicAffix, ID2Data
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
