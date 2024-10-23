using D2Shared.Huffman;
using D2SImporter.Model;
using System.Collections.Generic;

namespace D2SImporter
{
    public interface IImporter
    {
        //public List<Unique> Uniques { get; set; }
        //public List<Runeword> Runewords { get; set; }
        //public List<CubeRecipe> CubeRecipes { get; set; }
        //public List<Set> Sets { get; set; }
        public Dictionary<int, Unique> Uniques { get; set; }
        public Dictionary<int, Runeword> Runewords { get; set; }
        public Dictionary<string, CubeRecipe> CubeRecipes { get; set; }
        public Dictionary<string, Set> Sets { get; set; }


        public Table Table { get; set; }
        public Dictionary<int, MagicAffix> MagicAffixes { get; set; }
        public Dictionary<int, MagicPrefix> MagicPrefixes { get; set; }
        public Dictionary<int, MagicSuffix> MagicSuffixes { get; set; }
        public Dictionary<string, ItemStatCost> ItemStatCosts { get; set; }
        public Dictionary<string, EffectProperty> EffectProperties { get; set; }
        public Dictionary<string, ItemType> ItemTypes { get; set; }
        public Dictionary<string, Armor> Armors { get; set; }
        public Dictionary<string, Weapon> Weapons { get; set; }
        public Dictionary<string, Skill> Skills { get; set; }
        public Dictionary<int, RarePrefix> RarePrefixes { get; set; }
        public Dictionary<int, RareSuffix> RareSuffixes { get; set; }
        public Dictionary<string, CharStat> CharStats { get; set; }
        public Dictionary<string, MonStat> MonStats { get; set; }
        public Dictionary<string, Misc> Miscs { get; set; }
        public Dictionary<string, Gem> Gems { get; set; }
        public Dictionary<string, SetItem> SetItems { get; set; }
        public Dictionary<int, string> SkillTabs { get; set; }

        public void LoadData();
        public void ImportModel();
        public void Export();

        public Skill? GetSkill(string skill);
        public Equipment? GetByCode(string code);
        public MagicAffix? GetMagicAffixById(int id);
        public RareAffix? GetRareAffixById(int id);
        public string ReplaceItemName(string item);

        public bool IsStackable(string code);
        public bool IsArmor(string code);
        public bool IsWeapon(string code);
        public bool IsMisc(string code);

        private static HuffmanTree? _itemCodeTree = null;
        private static HuffmanTree InitializeHuffmanTree()
        {
            HuffmanTree itemCodeTree = new();
            itemCodeTree.Build();
            return itemCodeTree;
        }

        internal static HuffmanTree ItemCodeTree
        {
            get => _itemCodeTree ??= InitializeHuffmanTree();
            set => _itemCodeTree = value;
        }
    }
}
