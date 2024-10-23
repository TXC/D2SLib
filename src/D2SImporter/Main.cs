using D2Shared.Huffman;
using D2SImporter.Model;

namespace D2SImporter
{
    public class Main
    {
        private static IImporter? _importer = null;
        public static IImporter Importer
        {
            get => _importer ??= new VersionedImporter();
            set => _importer = value;
        }

        #region Helpers
        internal static HuffmanTree ItemCodeTree => IImporter.ItemCodeTree;

        public static Skill? GetSkill(string skill) => Importer.GetSkill(skill);

        public static Equipment? GetByCode(string code) => Importer.GetByCode(code);

        public static MagicAffix? GetMagicAffixById(int id) => Importer.GetMagicAffixById(id);

        public static RareAffix? GetRareAffixById(int id) => Importer.GetRareAffixById(id);

        public static string ReplaceItemName(string item) => Importer.ReplaceItemName(item);

        public static bool IsStackable(string code) => Importer.IsStackable(code.TrimEnd());

        public static bool IsArmor(string code) => Importer.IsArmor(code.TrimEnd());

        public static bool IsWeapon(string code) => Importer.IsWeapon(code.TrimEnd());

        public static bool IsMisc(string code) => Importer.IsMisc(code.TrimEnd());
        #endregion Helpers
    }
}
