using D2SImporter.Model;
using System;
using System.IO;
using System.Linq;

namespace D2SImporter
{
    public class FromPathImporter : BaseImporter, IImporter
    {
        private readonly string _outputPath;
        private readonly string _excelPath;
        private readonly string _tablePath;

        public FromPathImporter()
        {
            throw new NotSupportedException();
        }

        public FromPathImporter(string excelPath, string tablePath) : base()
        {
            if (!Directory.Exists(excelPath))
            {
                throw new Exception($"Could not find excel directory at '{_excelPath}'");
            }

            if (!Directory.Exists(tablePath))
            {
                throw new Exception($"Could not find table directory at '{_tablePath}'");
            }

            _excelPath = excelPath.Trim('/', '\\');
            _tablePath = tablePath.Trim('/', '\\');
            _outputPath = string.Empty;
        }

        public FromPathImporter(string excelPath, string tablePath, string outputDir) : this(excelPath, tablePath)
        {
            if (!Directory.Exists(outputDir))
            {
                throw new Exception($"Could not find output directory at '{outputDir}'");
            }

            _outputPath = outputDir.Trim('/', '\\');
        }

        public void ImportModel()
        {
            if (string.IsNullOrEmpty(_excelPath))
            {
                throw new Exception("Invalid excel path");
            }

            try
            {
                Uniques = ImportFromFile<int, Unique>(this, _excelPath);
                Runewords = ImportFromFile<int, Runeword>(this, _excelPath);
                CubeRecipes = ImportFromFile<CubeRecipe>(this, _excelPath);
                Sets = ImportFromFile<Set>(this, _excelPath);
            }
            catch (Exception e)
            {
                ExceptionHandler.WriteException(e);
            }
        }

        public void LoadData()
        {
            if (string.IsNullOrEmpty(_tablePath))
            {
                throw new Exception("Invalid table path");
            }

            if (string.IsNullOrEmpty(_excelPath))
            {
                throw new Exception("Invalid excel path");
            }

            try
            {
                Table = Table.ImportFromTbl(_tablePath);

                MagicSuffixes = ImportFromFile<int, MagicSuffix>(this, _excelPath);
                MagicPrefixes = ImportFromFile<int, MagicPrefix>(this, _excelPath);
                MagicAffixes = ImportFromFile<int, MagicAffix>(this, _excelPath);
                ItemStatCosts = ImportFromFile<ItemStatCost>(this, _excelPath);
                EffectProperties = ImportFromFile<EffectProperty>(this, _excelPath);
                ItemTypes = ImportFromFile<ItemType>(this, _excelPath);
                Armors = ImportFromFile<Armor>(this, _excelPath);
                Weapons = ImportFromFile<Weapon>(this, _excelPath);
                Skills = ImportFromFile<Skill>(this, _excelPath);
                RarePrefixes = ImportFromFile<int, RarePrefix>(this, _excelPath);
                RareSuffixes = ImportFromFile<int, RareSuffix>(this, _excelPath);
                CharStats = ImportFromFile<CharStat>(this, _excelPath);
                MonStats = ImportFromFile<MonStat>(this, _excelPath);
                Miscs = ImportFromFile<Misc>(this, _excelPath);
                Gems = ImportFromFile<Gem>(this, _excelPath);
                SetItems = ImportFromFile<SetItem>(this, _excelPath);
            }
            catch (Exception e)
            {
                ExceptionHandler.WriteException(e);
            }
        }

        public override void Export()
        {
            if (string.IsNullOrEmpty(_outputPath))
            {
                throw new Exception("Invalid output path");
            }

            try
            {
                //TxtExporter.ExportTxt(_outputPath, Uniques, Runewords, CubeRecipes, Sets); // Out of date
                JsonExporter.ExportJson(_outputPath,
                    Uniques.Select(x => x.Value).ToList(),
                    Runewords.Select(x => x.Value).ToList(),
                    CubeRecipes.Select(x => x.Value).ToList(),
                    Sets.Select(x => x.Value).ToList()
                    );
                WebExporter.ExportWeb(_outputPath);
            }
            catch (Exception e)
            {
                ExceptionHandler.WriteException(e);
            }
        }
    }
}
