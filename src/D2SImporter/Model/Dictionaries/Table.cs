using D2Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace D2SImporter.Model
{
    public class Table
    {
        [JsonIgnore]
        private Dictionary<string, int> Index = [];

        [JsonIgnore]
        private Dictionary<int, TableList> Data = [];

        public int Count { get => Data.Count; }

        public static Table ImportFromTxt(string tableFolder)
        {
            Table table = new();

            var files = Directory.GetFiles(tableFolder, "*.txt");

            var tmpFiles = files.ToList();
            tmpFiles.Sort(SortResources);
            files = [.. tmpFiles];

            int offset = 0;
            foreach (var file in files)
            {
                var lines = Utility.ReadTxtFileToList(file);
                table.HandleTextFiles(lines, offset);
                offset += 10000;
            }
            return table;
        }

        public static Table ImportFromTxtAssembly(string version = "96")
        {
            Table table = new();

            var assembly = Assembly.GetExecutingAssembly();
            string[] files = [.. assembly.GetManifestResourceNames().Where(x => x.StartsWith($"D2SImporter.Resources.Table.{version}") && x.EndsWith(".txt"))];

            var tmpFiles = files.ToList();
            tmpFiles.Sort(SortResources);
            files = [.. tmpFiles];

            int offset = 0;
            foreach (var file in files)
            {
                var lines = Utility.ReadAssemblyFileToList(file);
                table.HandleTextFiles(lines, offset);
                offset += 10000;
            }

            return table;
        }

        public static Table ImportFromTbl(string tableFolder)
        {
            Table table = new();

            var files = Directory.GetFiles(tableFolder, "*.tbl");

            if (files.Length == 0)
            {
                ExceptionHandler.LogException(new Exception($"Could not find any .tbl files in '{tableFolder}'"));
            }

            var tmpFiles = files.ToList();
            tmpFiles.Sort(SortResources);
            files = [.. tmpFiles];

            int offset = 0;
            foreach (var file in files)
            {
                var hashTable = TableProcessor.ReadTablesFile(file, offset);

                table.HandleHashTable(hashTable);
                //hashTable.ForEach(table.AddDataRow);

                offset += 10000;
            }

            table.FixBrokenValues();
            return table;
        }

        public static Table ImportFromTblAssembly(string version = "96")
        {
            Table table = new();

            var assembly = Assembly.GetExecutingAssembly();
            var res = assembly.GetManifestResourceNames();
            string[] files = [.. res.Where(x => x.StartsWith($"D2SImporter.Resources._{version}.Table") && x.EndsWith(".tbl"))];

            if (files.Length == 0)
            {

                ExceptionHandler.LogException(new Exception($"Could not find any .tbl files in assembly"));
            }

            var tmpFiles = files.ToList();
            tmpFiles.Sort(SortResources);
            files = [.. tmpFiles];

            int offset = 0;
            foreach (var file in files)
            {
                using Stream resource = assembly.GetManifestResourceStream(file);
                var hashTable = TableProcessor.ReadTablesFile(resource, offset);

                table.HandleHashTable(hashTable);
                //hashTable.ForEach(table.AddDataRow);

                offset += 10000;
            }

            table.FixBrokenValues();
            return table;
        }

        void HandleTextFiles(List<string> lines, int offset = 0)
        {
            var tableList = new List<TableList>();
            for (int i = 0; i < lines.Count; i++)
            {
                var values = lines[i].Split('\t');

                var key = values[0].Trim('"');
                var value = values[1].Trim('"');

                if (string.IsNullOrEmpty(key))
                {
                    continue;
                }

                tableList.Add(new() { Key = key, Value = value, Index = offset + i });
            }
            HandleHashTable(tableList);
        }

        void HandleHashTable(List<TableList> hashTable)
        {
            hashTable.ToDictionary(x => x.Index, x => x)
                     .ToList()
                     .ForEach(x => Data.Add(x.Key, x.Value));

            hashTable.Where(x => !x.Key.Equals("x", StringComparison.CurrentCultureIgnoreCase))
                     .ToList()
                     .ForEach(UpdateIndex);
        }

        void UpdateIndex(TableList row)
        {
            if (!Index.TryAdd(row.Key, row.Index))
            {
                Index[row.Key] = row.Index;
            }
        }

        static int SortResources(string x, string y)
        {
            // e - expansion
            // p - patch
            // s - original
            int pos1 = x.LastIndexOf('.', x.Length - 5) + 1;
            int pos2 = y.LastIndexOf('.', y.Length - 5) + 1;

            if (x.Substring(pos1, 1) == "e")
            {
                return y.Substring(pos2, 1) switch
                {
                    "e" => 0,
                    "p" => -1,
                    "s" => 1,
                    _ => throw new Exception($"Unexpected value: {y}")
                };
            }
            else if (x.Substring(pos1, 1) == "p")
            {
                return y.Substring(pos2, 1) switch
                {
                    "e" => 1,
                    "p" => 0,
                    "s" => 1,
                    _ => throw new Exception($"Unexpected value: {y}")
                };
            }
            else if (x.Substring(pos1, 1) == "s")
            {
                return y.Substring(pos2, 1) switch
                {
                    "e" => -1,
                    "p" => -1,
                    "s" => 0,
                    _ => throw new Exception($"Unexpected value: {y}")
                };
            }
            throw new Exception($"Unexpected value: {x} / {y}");
        }

        public string GetValue(string key)
        {
            if (Index.TryGetValue(key, out int index))
            {
                if (!Data.TryGetValue(index, out TableList tableList))
                {
                    ExceptionHandler.LogException(new Exception($"Could not find key '{key}' with index '{index}' in any .tbl file"));
                }
                string value = tableList.Value;

                // Fix class skills
                if (key == "ModStr3a")
                {
                    value = value.Replace("Amazon", "%d");
                }

                return value;
            }

            return key;
        }

        public bool TryGetValue(string key, out string value)
        {
            try
            {
                value = GetValue(key);
                return true;
            }
            catch (Exception)
            {
                value = default;
            }
            return false;
        }

        void FixBrokenValues()
        {
            var hashTable = new List<TableList>
            {
                new() {Key = "StrSklTabItem1", Value = "+%d to Javelin and Spear Skills", Index = 0x7530},
                new() {Key = "StrSklTabItem2", Value = "+%d to Passive and Magic Skills", Index = 0x7531},
                new() {Key = "StrSklTabItem3", Value = "+%d to Bow and Crossbow Skills", Index = 0x7532},
                new() {Key = "StrSklTabItem4", Value = "+%d to Defensive Auras", Index = 0x7533},
                new() {Key = "StrSklTabItem5", Value = "+%d to Offensive Auras", Index = 0x7534},
                new() {Key = "StrSklTabItem6", Value = "+%d to Combat Skills", Index = 0x7535},
                new() {Key = "StrSklTabItem7", Value = "+%d to Summoning Skills", Index = 0x7536},
                new() {Key = "StrSklTabItem8", Value = "+%d to Poison and Bone Skills", Index = 0x7537},
                new() {Key = "StrSklTabItem9", Value = "+%d to Curses", Index = 0x7538},
                new() {Key = "StrSklTabItem10", Value = "+%d to Warcries", Index = 0x7539},
                new() {Key = "StrSklTabItem11", Value = "+%d to Combat Skills", Index = 0x753a},
                new() {Key = "StrSklTabItem12", Value = "+%d to Masteries", Index = 0x753b},
                new() {Key = "StrSklTabItem13", Value = "+%d to Cold Skills", Index = 0x753c},
                new() {Key = "StrSklTabItem14", Value = "+%d to Lightning Skills", Index = 0x753d},
                new() {Key = "StrSklTabItem15", Value = "+%d to Fire Skills", Index = 0x753e},
                new() {Key = "StrSklTabItem16", Value = "+%d to Summoning Skills", Index = 0x753f},
                new() {Key = "StrSklTabItem17", Value = "+%d to Shape Shifting Skills", Index = 0x7540},
                new() {Key = "StrSklTabItem18", Value = "+%d to Elemental Skills", Index = 0x7541},
                new() {Key = "StrSklTabItem19", Value = "+%d to Traps", Index = 0x7542},
                new() {Key = "StrSklTabItem20", Value = "+%d to Shadow Disciplines", Index = 0x7543},
                new() {Key = "StrSklTabItem21", Value = "+%d to Martial Arts", Index = 0x7544},
                // Missing posts
                new() {Key = "ModStr2uPercent", Value = "Physical Damage Received Reduced by %d%%", Index = 0x7545},
                new() {Key = "ModStr2uPercentNegative", Value = "Physical Damage Received Increased by %d%%", Index = 0x7546},
                new() {Key = "ItemModifierNonClassSkill", Value = "%+d to %s", Index = 0x7547},
                new() {Key = "ItemModifierClassSkill", Value = "%+d to %s %s", Index = 0x7548},
                new() {Key = "ModItemMonColdSunder", Value = "Monster Cold Immunity is Sundered", Index = 0x7549},
                new() {Key = "ModItemMonFireSunder", Value = "Monster Fire Immunity is Sundered", Index = 0x754a},
                new() {Key = "ModItemMonLightSunder", Value = "Monster Lightning Immunity is Sundered", Index = 0x754b},
                new() {Key = "ModItemMonPoisonSunder", Value = "Monster Poison Immunity is Sundered", Index = 0x754c},
                new() {Key = "ModItemMonPhysicalSunder", Value = "Monster Physical Immunity is Sundered", Index = 0x754d },
                new() {Key = "ModItemMonMagicSunder", Value = "Monster Magic Immunity is Sundered", Index = 0x754e},
                new() {Key = "ModStr6j", Value = "%+d%% Chance to not consume Quantity", Index = 0x754f},
                new() {Key = "ModStr6k", Value = "%+d%% chance for finishing moves to not consume charges", Index = 0x7550},
                new() {Key = "ModStr6l", Value = "%+d%% Attack Speed", Index = 0x7551},
            };

            HandleHashTable(hashTable);
        }
    }
}
