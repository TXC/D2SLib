using D2SImporter.Model;
using D2Shared.Enums;
using System;
using System.Collections.Generic;

namespace D2SImporter
{
    public class VersionedImporter : IImporter
    {
        private Dictionary<SaveVersion, FromAssemblyImporter> Versions = [];
        private SaveVersion CurrentVersion = SaveVersion.v11x;

        private Dictionary<string, SaveVersion> AvailableVersions = new()
        {
            {"96", SaveVersion.v11x},
            {"97", SaveVersion.v11x},
            {"98", SaveVersion.v11x},
            {"99", SaveVersion.v250},
        };

        public Dictionary<int, Unique> Uniques
        {
            get => Versions[CurrentVersion].Uniques;
            set => Versions[CurrentVersion].Uniques = value;
        }
        public Dictionary<int, Runeword> Runewords
        {
            get => Versions[CurrentVersion].Runewords;
            set => Versions[CurrentVersion].Runewords = value;
        }
        public Dictionary<string, CubeRecipe> CubeRecipes
        {
            get => Versions[CurrentVersion].CubeRecipes;
            set => Versions[CurrentVersion].CubeRecipes = value;
        }
        public Dictionary<string, Set> Sets
        {
            get => Versions[CurrentVersion].Sets;
            set => Versions[CurrentVersion].Sets = value;
        }

        public Table Table
        {
            get => Versions[CurrentVersion].Table;
            set => Versions[CurrentVersion].Table = value;
        }
        public Dictionary<int, MagicSuffix> MagicSuffixes
        {
            get => Versions[CurrentVersion].MagicSuffixes;
            set => Versions[CurrentVersion].MagicSuffixes = value;
        }
        public Dictionary<int, MagicPrefix> MagicPrefixes
        {
            get => Versions[CurrentVersion].MagicPrefixes;
            set => Versions[CurrentVersion].MagicPrefixes = value;
        }
        public Dictionary<int, MagicAffix> MagicAffixes
        {
            get => Versions[CurrentVersion].MagicAffixes;
            set => Versions[CurrentVersion].MagicAffixes = value;
        }

        public Dictionary<string, ItemStatCost> ItemStatCosts
        {
            get => Versions[CurrentVersion].ItemStatCosts;
            set => Versions[CurrentVersion].ItemStatCosts = value;
        }
        public Dictionary<string, EffectProperty> EffectProperties
        {
            get => Versions[CurrentVersion].EffectProperties;
            set => Versions[CurrentVersion].EffectProperties = value;
        }
        public Dictionary<string, ItemType> ItemTypes
        {
            get => Versions[CurrentVersion].ItemTypes;
            set => Versions[CurrentVersion].ItemTypes = value;
        }
        public Dictionary<string, Armor> Armors
        {
            get => Versions[CurrentVersion].Armors;
            set => Versions[CurrentVersion].Armors = value;
        }
        public Dictionary<string, Weapon> Weapons
        {
            get => Versions[CurrentVersion].Weapons;
            set => Versions[CurrentVersion].Weapons = value;
        }
        public Dictionary<string, Skill> Skills
        {
            get => Versions[CurrentVersion].Skills;
            set => Versions[CurrentVersion].Skills = value;
        }
        public Dictionary<int, RarePrefix> RarePrefixes
        {
            get => Versions[CurrentVersion].RarePrefixes;
            set => Versions[CurrentVersion].RarePrefixes = value;
        }
        public Dictionary<int, RareSuffix> RareSuffixes
        {
            get => Versions[CurrentVersion].RareSuffixes;
            set => Versions[CurrentVersion].RareSuffixes = value;
        }
        public Dictionary<string, CharStat> CharStats
        {
            get => Versions[CurrentVersion].CharStats;
            set => Versions[CurrentVersion].CharStats = value;
        }
        public Dictionary<string, MonStat> MonStats
        {
            get => Versions[CurrentVersion].MonStats;
            set => Versions[CurrentVersion].MonStats = value;
        }
        public Dictionary<string, Misc> Miscs
        {
            get => Versions[CurrentVersion].Miscs;
            set => Versions[CurrentVersion].Miscs = value;
        }
        public Dictionary<string, Gem> Gems
        {
            get => Versions[CurrentVersion].Gems;
            set => Versions[CurrentVersion].Gems = value;
        }
        public Dictionary<string, SetItem> SetItems
        {
            get => Versions[CurrentVersion].SetItems;
            set => Versions[CurrentVersion].SetItems = value;
        }
        public Dictionary<int, string> SkillTabs
        {
            get => Versions[CurrentVersion].SkillTabs;
            set => Versions[CurrentVersion].SkillTabs = value;
        }

        public VersionedImporter()
        {
            foreach (var avail in AvailableVersions)
            {
                if (Versions.ContainsKey(avail.Value))
                {
                    continue;
                }
                var cur = new FromAssemblyImporter()
                {
                    Version = avail.Value
                };
                Versions.Add(avail.Value, cur);
            }
        }

        private SaveVersion ValidateVersion(SaveVersion version)
        {
            var ver = ((int)version).ToString();
            return AvailableVersions[ver];
        }

        public void SetVersion(SaveVersion version)
            => CurrentVersion = ValidateVersion(version);

        public SaveVersion GetVersion()
            => CurrentVersion;

        public void ImportModel()
        {
            var old = CurrentVersion;
            foreach (var avail in Versions)
            {
                CurrentVersion = avail.Key;
                Versions[avail.Key].ImportModel();
            }
            CurrentVersion = old;
        }

        public void LoadData()
        {
            try
            {
                var old = CurrentVersion;
                foreach (var avail in Versions)
                {
                    CurrentVersion = avail.Key;
                    Versions[avail.Key].LoadData();
                }
                CurrentVersion = old;
            }
            catch (Exception e)
            {
                ExceptionHandler.WriteException(e);
            }
        }

        public void Export()
        {
            throw new NotImplementedException();
        }

        public Skill? GetSkill(string skill)
            => Versions[CurrentVersion].GetSkill(skill);

        public Equipment? GetByCode(string code)
            => Versions[CurrentVersion].GetByCode(code.TrimEnd());

        public MagicAffix? GetMagicAffixById(int id)
            => Versions[CurrentVersion].GetMagicAffixById(id);

        public RareAffix? GetRareAffixById(int id)
            => Versions[CurrentVersion].GetRareAffixById(id);

        public string ReplaceItemName(string item)
            => Versions[CurrentVersion].ReplaceItemName(item);

        public bool IsStackable(string code)
            => Versions[CurrentVersion].IsStackable(code.TrimEnd());

        public bool IsArmor(string code)
            => Versions[CurrentVersion].IsArmor(code.TrimEnd());

        public bool IsWeapon(string code)
            => Versions[CurrentVersion].IsWeapon(code.TrimEnd());

        public bool IsMisc(string code)
            => Versions[CurrentVersion].IsMisc(code.TrimEnd());
    }
}
