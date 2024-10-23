using D2Shared;
using D2Shared.Enums;
using D2SImporter.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace D2SImporter.Model
{
    [FileName("CubeMain.txt")]
    [PreImportRow(typeof(Action<IImporter, CubeRecipe, Dictionary<string, string>>), typeof(CubeRecipe), nameof(PreRowImport))]
    [PostImportRow(typeof(Action<IImporter, CubeRecipe, Dictionary<string, string>>), typeof(CubeRecipe), nameof(PostRowImport))]
    public class CubeRecipe : ID2Data
    {
        public static bool UseDescription { get; set; }

        [ColumnName("enabled")]
        public bool Enabled { get; set; }

        [ColumnName("description")]
        [ComplexImport(typeof(Func<IImporter, Dictionary<string, string>, string>), typeof(CubeRecipe), nameof(GetDescription))]
        [Key]
        public string Description { get; set; }

        public string Item { get; set; }

        [JsonIgnore]
        public List<string> InputList { get; set; }

        public string Output { get; set; }

        public string Input { get; set; }

        public string CubeRecipeDescription { get; set; }

        [ComplexImport(typeof(Func<Dictionary<string, string>, List<CubeMod>>), typeof(CubeRecipe), nameof(GetModifiers))]
        public List<CubeMod> Modifiers { get; set; } = [];

        [JsonIgnore]
        public List<CubeRecipe> Data { get; set; } = [];

        public override string ToString()
        {
            return Description;
        }

        public CubeRecipe()
        {

        }

        public CubeRecipe(IImporter importer, string[] inputArray, string output)
        {
            InputList = new List<string>();
            foreach (var input in inputArray)
            {
                InputList.Add(GetOutput(importer, input));
            }

            Output = GetOutput(importer, output);
        }

        static string GetOutput(IImporter importer, string value)
        {
            var valueParams = value.Replace("\"", "").Split(',');
            var item = importer.ReplaceItemName(valueParams[0]);

            //if (importer.Table.TryGetValue(item, out string itemValue))
            //{
            //    item = itemValue;
            //}

            var parameters = valueParams.Skip(1).ToArray();
            var parameterString = GetParameterString(importer, parameters);

            var valueResult = parameterString.Replace("%d", item);

            return valueResult;
        }

        static string[] GetInput(Dictionary<string, string> row)
        {
            // Input
            var numInputs = Utility.ToNullableInt(row["numinputs"]);
            if (numInputs is null)
            {
                throw new Exception($"Cube recipe '{row["description"]}' does not have a numinputs");
            }

            var inputArray = new List<string>();
            for (int i = 1; i <= numInputs.Value; i++)
            {
                inputArray.Add(row[$"input {i}"]);
            }
            inputArray.RemoveAll(x => string.IsNullOrEmpty(x));
            return [.. inputArray];
        }

        static List<CubeMod> GetModifiers(Dictionary<string, string> row)
        {
            var modifiers = new List<CubeMod>();
            for (int i = 1; i <= 5; i++)
            {
                if (!string.IsNullOrEmpty(row[$"mod {i}"]))
                {
                    modifiers.Add(new CubeMod
                    {
                        Mod = row[$"mod {i}"],
                        ModChance = row[$"mod {i} chance"],
                        ModParam = row[$"mod {i} param"],
                        ModMin = row[$"mod {i} min"],
                        ModMax = row[$"mod {i} max"]
                    });
                }
            }
            return modifiers;
        }

        static void PreRowImport(IImporter importer, CubeRecipe recipe, Dictionary<string, string> row)
        {
            // Input
            var inputArray = GetInput(row);

            recipe.InputList = [];
            foreach (var input in inputArray)
            {
                recipe.InputList.Add(GetOutput(importer, input));
            }

            recipe.Output = GetOutput(importer, row["output"]);


            //return new CubeRecipe(importer, inputArray.ToArray(), row["output"]);
        }

        static void PostRowImport(IImporter importer, CubeRecipe recipe, Dictionary<string, string> row)
        {
            var inputArray = GetInput(row);
            var modifiers = GetModifiers(row);
            if (recipe.Output.Contains("usetype"))
            {
                var type = inputArray[0].Replace("\"", "").Split(',')[0];
                if (importer.ItemTypes.TryGetValue(type, out ItemType? value))
                {
                    recipe.Output = recipe.Output.Replace("usetype", value.Name);
                }
                else if (type == "any")
                {
                    recipe.Output = recipe.Output.Replace("usetype", "item");
                }
                else
                {
                    recipe.Output = recipe.Output.Replace("usetype", recipe.InputList[0]);
                }
            }

            if (recipe.Output.Contains("useitem"))
            {
                var item = inputArray[0].Replace("\"", "").Split(',')[0];
                if (importer.ItemTypes.TryGetValue(item, out ItemType? value))
                {
                    recipe.Output = recipe.Output.Replace("useitem", value.Name);
                }
                else
                {
                    recipe.Output = recipe.Output.Replace("useitem", "");
                }

                if (modifiers.Count > 0 && modifiers[0].Mod == "sock")
                {
                    recipe.Output = $"Socketed {recipe.Output}";
                }
            }

            recipe.Input = "";
            foreach (var input in recipe.InputList)
            {
                recipe.Input += $"{input} + ";
            }
            recipe.Input = recipe.Input.Substring(0, recipe.Input.LastIndexOf('+'));

            if (UseDescription)
            {
                recipe.CubeRecipeDescription = GetDescription(importer, row);
            }
            else
            {
                recipe.CubeRecipeDescription = $"{recipe.Input} = {recipe.Output}";
            }

            recipe.CubeRecipeDescription = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(recipe.CubeRecipeDescription);
        }

        static string GetDescription(IImporter importer, Dictionary<string, string> row)
        {
            string descr = row["description"].Replace("rune ", "r");
            var matches = Regex.Matches(descr, @"(r\d\d)");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    if (importer.Miscs.TryGetValue(match.Groups[1].Value, out Misc? rune))
                    {
                        descr = descr.Replace(rune.Code, rune.Name);
                    }
                }
            }
            return descr;
        }

        public static string GetParameterString(string[] parameters)
            => GetParameterString(Main.Importer, parameters);

        public static string GetParameterString(IImporter importer, string[] parameters)
        {
            var result = "%d";

            foreach (var parameter in parameters)
            {

                switch (parameter)
                {
                    case "low":
                        result = result.Replace("%d", "Low Quality %d");
                        break;
                    case "hig":
                        result = result.Replace("%d", "High Quality %d");
                        break;

                    case "nor":
                        result = result.Replace("%d", "Normal %d");
                        break;
                    case "mag":
                        // If the magic item has a sufix, don't add magic to it
                        if (parameters.Any(x => x.StartsWith("pre=") || x.StartsWith("suf=")))
                        {
                            continue;
                        }
                        result = result.Replace("%d", "Magic %d");
                        break;
                    case "rar":
                        result = result.Replace("%d", "Rare %d");
                        break;
                    case "set":
                        result = result.Replace("%d", "Set %d");
                        break;
                    case "uni":
                        result = result.Replace("%d", "Unique %d");
                        break;
                    case "orf":
                        result = result.Replace("%d", "Crafted %d");
                        break;
                    case "tmp":
                        result = result.Replace("%d", "Tempered %d");
                        break;

                    case "eth":
                        result = result.Replace("%d", "Etheral %d");
                        break;
                    case "noe":
                        result = result.Replace("%d", "Not Etheral %d");
                        break;

                    case "nos":
                        result = result.Replace("%d", "No Socket %d");
                        break;

                    case "pre": // todo
                        break;
                    case "suf": // todo
                        break;

                    case "rep":
                        result = result.Replace("%d", "%d Repair durability");
                        break;
                    case "rch":
                        result = result.Replace("%d", "%d Recharge Quantity");
                        break;

                    case "upg": // Include?
                        //result = result.Replace("%d", "%d upgraded");
                        break;
                    case "bas":
                        result = result.Replace("%d", "basic %d");
                        break;
                    case "exc":
                        result = result.Replace("%d", "exceptional %d");
                        break;
                    case "eli":
                        result = result.Replace("%d", "elite %d");
                        break;

                    case "sock":
                        result = result.Replace("%d", "Socketed %d");
                        break;

                    case "uns":
                        result = result.Replace("%d", "Destroy gems %d");
                        break;
                    case "rem":
                        result = result.Replace("%d", "Remove gems %d");
                        break;

                    case "req":
                        result = result.Replace("%d", "reroll item %d");
                        break;

                    default:
                        break;

                }

                if (parameter.StartsWith("qty="))
                {
                    var quantity = parameter.Replace("qty=", "");
                    result = result.Replace(result, $"{quantity} {result}");
                    continue;
                }
                else if (parameter.StartsWith("sock="))
                {
                    var quantity = parameter.Replace("sock=", "");
                    result = result.Replace("%d", $"{quantity} Sockets");
                    continue;
                }
                else if (parameter.StartsWith("pre="))
                {
                    var index = int.Parse(parameter.Replace("pre=", ""));
                    result = result.Replace("%d", $"{importer.MagicPrefixes[index].Name} %d");
                    continue;
                }
                else if (parameter.StartsWith("suf="))
                {
                    var index = int.Parse(parameter.Replace("suf=", ""));
                    result = result.Replace("%d", $"%d {importer.MagicSuffixes[index].Name}");
                    continue;
                }
            }

            return result;
        }
    }

    public class CubeMod
    {
        [ColumnName("mod {0}")]
        public string Mod { get; set; }

        [ColumnName("mod {0} chance")]
        public string ModChance { get; set; }

        [ColumnName("mod {0} param")]
        public string ModParam { get; set; }

        [ColumnName("mod {0} min")]
        public string ModMin { get; set; }

        [ColumnName("mod {0} max")]
        public string ModMax { get; set; }
    }
}
