using D2SImporter.Attributes;

namespace D2SImporter.Model
{
    [FileName("MagicPrefix.txt")]
    public class MagicPrefix : MagicAffix, ID2Data
    {
        public override string ToString()
        {
            return Name;
        }
    }
}
