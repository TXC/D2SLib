using D2Shared.Enums;
using D2SImporter.Model;
using System;

namespace D2SImporter
{
    public class FromAssemblyImporter : BaseImporter, IImporter
    {
        public SaveVersion Version { get; set; } = SaveVersion.v11x;

        public void ImportModel()
            => ImportModel(((int)Version).ToString());

        public void ImportModel(string version)
        {
            try
            {
                Uniques = ImportFromAssembly<int, Unique>(this, version);
                Runewords = ImportFromAssembly<int, Runeword>(this, version);
                CubeRecipes = ImportFromAssembly<CubeRecipe>(this, version);
                Sets = ImportFromAssembly<Set>(this, version);
            }
            catch (Exception e)
            {
                ExceptionHandler.WriteException(e);
            }
        }

        public void LoadData()
            => LoadData(((int)Version).ToString());

        public void LoadData(string version)
        {
            try
            {
                Table = Table.ImportFromTblAssembly(version);

                MagicSuffixes = ImportFromAssembly<int, MagicSuffix>(this, version);
                MagicPrefixes = ImportFromAssembly<int, MagicPrefix>(this, version);
                MagicAffixes = ImportFromAssembly<int, MagicAffix>(this, version);
                ItemStatCosts = ImportFromAssembly<ItemStatCost>(this, version);
                EffectProperties = ImportFromAssembly<EffectProperty>(this, version);
                ItemTypes = ImportFromAssembly<ItemType>(this, version);
                Armors = ImportFromAssembly<Armor>(this, version);
                Weapons = ImportFromAssembly<Weapon>(this, version);
                Miscs = ImportFromAssembly<Misc>(this, version);
                Skills = ImportFromAssembly<Skill>(this, version);
                RarePrefixes = ImportFromAssembly<int, RarePrefix>(this, version);
                RareSuffixes = ImportFromAssembly<int, RareSuffix>(this, version);
                CharStats = ImportFromAssembly<CharStat>(this, version);
                MonStats = ImportFromAssembly<MonStat>(this, version);
                Gems = ImportFromAssembly<Gem>(this, version);
                SetItems = ImportFromAssembly<SetItem>(this, version);
            }
            catch (Exception e)
            {
                ExceptionHandler.WriteException(e);
            }
        }
    }
}
