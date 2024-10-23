using D2Shared;
using D2SImporter.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace D2SImporter.Model
{
    public struct ItemPropertyStatCount
    {
        public int NumberOfProperties { get; set; }
        public string RangeStr { get; set; }
        public string EqualStr { get; set; }
    }

    [FileName("ItemStatCost.txt"), PostImport(typeof(Action<Dictionary<string, ItemStatCost>>), typeof(ItemStatCost), nameof(PostImport))]
    public class ItemStatCost : ID2Data
    {
        #region Properties
        [JsonIgnore, ColumnName, Key]
        public string Stat { get; set; }

        [JsonIgnore, ColumnName("*ID")]
        public int Id { get; set; }

        [JsonIgnore, ColumnName("Send Other")]
        public bool SendOther { get; set; }

        [JsonIgnore, ColumnName("Signed")]
        public bool Signed { get; set; }

        [JsonIgnore, ColumnName("Send Bits")]
        public int? SendBits { get; set; }

        [JsonIgnore, ColumnName("Send Param Bits")]
        public int? SendParamBits { get; set; }

        [JsonIgnore, ColumnName("UpdateAnimRate")]
        public bool UpdateAnimRate { get; set; }

        [JsonIgnore, ColumnName("Saved")]
        public bool Saved { get; set; }

        [JsonIgnore, ColumnName("CSvSigned")]
        public bool CSvSigned { get; set; }

        [JsonIgnore, ColumnName("CSvBits")]
        public int? CSvBits { get; set; }

        [JsonIgnore, ColumnName("CSvParam")]
        public int? CSvParam { get; set; }

        [JsonIgnore, ColumnName("fCallback")]
        public bool FuncCallback { get; set; }

        [JsonIgnore, ColumnName("fMin")]
        public bool FuncMin { get; set; }

        [JsonIgnore, ColumnName("MinAccr")]
        public int? MinAccr { get; set; }

        [JsonIgnore, ColumnName("Encode")]
        public int Encode { get; set; }

        [JsonIgnore, ColumnName("Add")]
        public int? Add { get; set; }

        [JsonIgnore, ColumnName("Multiply")]
        public int? Multiply { get; set; }

        [JsonIgnore, ColumnName("ValShift")]
        public int? ValShift { get; set; }

        [JsonIgnore, ColumnName("1.09-Save Bits")]
        public int? V109SaveBits { get; set; }

        [JsonIgnore, ColumnName("1.09-Save Add")]
        public int? V109SaveAdd { get; set; }

        [JsonIgnore, ColumnName("Save Bits")]
        public int? SaveBits { get; set; }

        [JsonIgnore, ColumnName("Save Add")]
        public int? SaveAdd { get; set; }

        [JsonIgnore, ColumnName("Save Param Bits")]
        public int SaveParamBits { get; set; }

        [JsonIgnore, ColumnName("keepzero")]
        public bool Keepzero { get; set; }

        [JsonIgnore, ColumnName("op")]
        public int? Op { get; set; }

        [JsonIgnore, ColumnName("op param")]
        public int? OpParam { get; set; }

        [JsonIgnore, ColumnName("op base")]
        public int? OpBase { get; set; }

        [JsonIgnore, ColumnName("op stat1")]
        public string OpStat1 { get; set; }

        [JsonIgnore, ColumnName("op stat2")]
        public string OpStat2 { get; set; }

        [JsonIgnore, ColumnName("op stat3")]
        public string OpStat3 { get; set; }

        [JsonIgnore, ColumnName("direct")]
        public bool Direct { get; set; }

        [JsonIgnore, ColumnName("maxstat")]
        public string MaxStat { get; set; }

        [JsonIgnore, ColumnName("damagerelated")]
        public bool DamageRelated { get; set; }

        [JsonIgnore, ColumnName("itemevent1")]
        public string ItemEvent1 { get; set; }

        [JsonIgnore, ColumnName("itemeventfunc1")]
        public int? ItemEventFunc1 { get; set; }

        [JsonIgnore, ColumnName("itemevent2")]
        public string ItemEvent2 { get; set; }

        [JsonIgnore, ColumnName("itemeventfunc2")]
        public int? ItemEventFunc2 { get; set; }

        [JsonIgnore, ColumnName("descpriority")]
        public int? DescriptionPriority { get; set; }

        [JsonIgnore, ColumnName("descfunc")]
        public int? DescriptionFunction { get; set; }

        [JsonIgnore, ColumnName("descval")]
        public int? DescriptionValue { get; set; }

        [JsonIgnore, ColumnName("descstrpos"), Translatable]
        public string DescriptionStringPositive { get; set; }

        [JsonIgnore]
        public string DescriptionStringRange { get; set; }

        [JsonIgnore]
        public string DescriptionStringEqual { get; set; }

        [JsonIgnore]
        public int NumberOfProperties { get; set; }

        [JsonIgnore, ColumnName("descstrneg"), Translatable]
        public string DescriptionStringNegative { get; set; }

        [JsonIgnore, ColumnName("descstr2"), Translatable]
        public string DescriptionString2 { get; set; }

        [JsonIgnore, ColumnName("dgrp")]
        public int? GroupDescription { get; set; }

        [JsonIgnore, ColumnName("dgrpfunc")]
        public int? GroupDescriptionFunction { get; set; }

        [JsonIgnore, ColumnName("dgrpval")]
        public int? GroupDescriptionValue { get; set; }

        [JsonIgnore, ColumnName("dgrpstrpos"), Translatable]
        public string GroupDescriptonStringPositive { get; set; }

        [JsonIgnore, ColumnName("dgrpstrneg"), Translatable]
        public string GroupDescriptionStringNegative { get; set; }

        [JsonIgnore, ColumnName("dgrpstr2"), Translatable]
        public string GroupDescriptionString2 { get; set; }

        [JsonIgnore]
        public Dictionary<string, ItemStatCost> Data { get; set; } = [];
        #endregion Properties

        static void PostImport(Dictionary<string, ItemStatCost> Items)
        {
            HardcodedTableStats(Items);
            FixBrokenEntries(Items);
            ApplyStatCount(Items);
        }

        public override string ToString()
        {
            return Stat;
        }

        static void HardcodedTableStats(Dictionary<string, ItemStatCost> Items)
        {
            var enhancedDamage = new ItemStatCost
            {
                Stat = "dmg%",
                DescriptionPriority = 144, // 1 below attack speed (seems right)
                DescriptionFunction = 4, // +val%
                DescriptionStringPositive = "Enhanced Damage",
                DescriptionStringNegative = "Enhanced Damage",
                DescriptionValue = 1 // Add value before
            };

            Items.Add(enhancedDamage.Stat, enhancedDamage);

            var ethereal = new ItemStatCost
            {
                Stat = "ethereal",
                DescriptionPriority = 1, // Min priority
                DescriptionFunction = 1, // lstValue
                DescriptionStringPositive = "Ethereal (Cannot Be Repaired)",
                DescriptionStringNegative = "Ethereal (Cannot Be Repaired)",
                DescriptionValue = 0 // Do not add value
            };

            Items.Add(ethereal.Stat, ethereal);

            var eledam = new ItemStatCost
            {
                Stat = "eledam",
                DescriptionPriority = Items["firemindam"].DescriptionPriority,
                DescriptionFunction = 30,
                DescriptionStringPositive = "Adds %d %s damage",
                DescriptionValue = 3
            };

            Items.Add(eledam.Stat, eledam);

            var resAll = new ItemStatCost
            {
                Stat = "res-all",
                DescriptionPriority = 34,
                DescriptionFunction = 4, // lstValue
                DescriptionStringPositive = "strModAllResistances",
                DescriptionStringNegative = "strModAllResistances",
                DescriptionValue = 2 // Do not add value
            };

            Items.Add(resAll.Stat, resAll);
        }

        static void FixBrokenEntries(Dictionary<string, ItemStatCost> Items)
        {
            if (Items.TryGetValue("item_numsockets", out ItemStatCost sockets))
            {
                sockets.DescriptionPriority = 1;
                sockets.DescriptionFunction = 29;
                sockets.DescriptionStringPositive = "Socketed";
                sockets.GroupDescriptionStringNegative = "Socketed";
                sockets.DescriptionValue = 3; // Use value as is
            }
        }

        static void ApplyStatCount(Dictionary<string, ItemStatCost> Items)
        {
            Dictionary<string, ItemPropertyStatCount> ItemStatCount = new() {
                {
                    "item_maxdamage_percent", new() {
                        NumberOfProperties = 2,
                        RangeStr = "strModMinDamageRange",
                        EqualStr = "strModEnhancedDamage"
                    }
                },
                {
                    "firemindam", new () {
                        NumberOfProperties = 2,
                        RangeStr = "strModFireDamageRange",
                        EqualStr = "strModFireDamage",
                    }
                },
                {
                    "lightmindam", new () {
                        NumberOfProperties = 2,
                        RangeStr = "strModLightningDamageRange",
                        EqualStr = "strModLightningDamage",
                    }
                },
                {
                    "magicmindam", new () {
                        NumberOfProperties = 2,
                        RangeStr = "strModMagicDamageRange",
                        EqualStr = "strModMagicDamage",
                    }
                },
                {
                    "coldmindam", new () {
                        NumberOfProperties = 3,
                        RangeStr = "strModColdDamageRange",
                        EqualStr = "strModColdDamage",
                    }
                },
                {
                    "poisonmindam", new () {
                        NumberOfProperties = 3,
                        RangeStr = "strModPoisonDamageRange",
                        EqualStr = "strModPoisonDamage",
                    }
                }
            };

            foreach (string key in Items.Keys)
            {
                if (ItemStatCount.TryGetValue(Items[key].Stat, out ItemPropertyStatCount stat))
                {
                    Items[key].DescriptionStringRange = stat.RangeStr;
                    Items[key].DescriptionStringEqual = stat.EqualStr;
                    Items[key].NumberOfProperties = stat.NumberOfProperties;
                }
                else
                {
                    Items[key].NumberOfProperties = 1;
                }
            }
        }

        public string PropertyString(IImporter importer, int? value, int? value2, string parameter, int itemLevel)
        {
            string lstValue;
            var valueString = GetValueString(value, value2);

            if (DescriptionStringPositive == null)
            {
                lstValue = Stat;
            }
            else if (value2.HasValue && value2.Value < 0)
            {
                lstValue = DescriptionStringNegative;
            }
            else
            {
                lstValue = DescriptionStringPositive;
            }

            if (DescriptionFunction.HasValue)
            {
                if (DescriptionFunction.Value >= 1 && DescriptionFunction.Value <= 4 && lstValue.Contains("%d"))
                {
                    valueString = lstValue.Replace("%d", valueString);
                    DescriptionValue = 3;
                }
                else
                {
                    CharStat? charStat;
                    switch (DescriptionFunction.Value)
                    {
                        case 1:
                            valueString = $"+{valueString}";
                            break;
                        case 2:
                            valueString = $"{valueString}%";
                            break;
                        case 3:
                            valueString = $"{valueString}";
                            break;
                        case 4:
                            valueString = $"+{valueString}%";
                            break;
                        case 5:
                            if (value.HasValue)
                            {
                                value = value * 100 / 128;
                            }
                            if (value2.HasValue)
                            {
                                value2 = value2 * 100 / 128;
                            }
                            valueString = GetValueString(value, value2);
                            valueString = $"{valueString}%";
                            break;
                        case 6:
                            double val1 = 0;
                            double val2 = 0;

                            if (value.HasValue && value2.HasValue)
                            {
                                val1 = CalculatePerLevel(value.Value.ToString(), Op, OpParam, Stat);
                                val2 = CalculatePerLevel(value2.Value.ToString(), Op, OpParam, Stat);
                            }
                            else
                            {
                                val1 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                                val2 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                            }

                            valueString = GetValueString(val1, val2);
                            lstValue = DescriptionStringPositive ?? "";
                            DescriptionValue = 3;
                            valueString = $"+({valueString} Per Character Level) {Math.Floor(val1).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val2 * 99).ToString(CultureInfo.InvariantCulture)} {lstValue} {DescriptionString2}";
                            break;
                        case 7:
                            val1 = 0;
                            val2 = 0;

                            if (value.HasValue && value2.HasValue)
                            {
                                val1 = CalculatePerLevel(value.Value.ToString(), Op, OpParam, Stat);
                                val2 = CalculatePerLevel(value2.Value.ToString(), Op, OpParam, Stat);
                            }
                            else
                            {
                                val1 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                                val2 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                            }

                            valueString = GetValueString(val1, val2);
                            lstValue = DescriptionStringPositive ?? "";
                            DescriptionValue = 3;
                            valueString = $"({valueString}% Per Character Level) {Math.Floor(val1).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val2 * 99).ToString(CultureInfo.InvariantCulture)}% {lstValue} (Based on Character Level)";
                            break;
                        case 8:
                            val1 = 0;
                            val2 = 0;

                            if (value.HasValue && value2.HasValue)
                            {
                                val1 = CalculatePerLevel(value.Value.ToString(), Op, OpParam, Stat);
                                val2 = CalculatePerLevel(value2.Value.ToString(), Op, OpParam, Stat);
                            }
                            else
                            {
                                val1 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                                val2 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                            }

                            valueString = GetValueString(val1, val2);
                            lstValue = DescriptionStringPositive ?? "";
                            DescriptionValue = 3;
                            valueString = $"+({valueString} Per Character Level) {Math.Floor(val1).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val2 * 99).ToString(CultureInfo.InvariantCulture)} {lstValue} (Based on Character Level)";
                            break;
                        case 9:
                            val1 = 0;
                            val2 = 0;

                            if (value.HasValue && value2.HasValue)
                            {
                                val1 = CalculatePerLevel(value.Value.ToString(), Op, OpParam, Stat);
                                val2 = CalculatePerLevel(value2.Value.ToString(), Op, OpParam, Stat);
                            }
                            else
                            {
                                val1 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                                val2 = CalculatePerLevel(parameter, Op, OpParam, Stat);
                            }

                            valueString = GetValueString(val1, val2);
                            lstValue = DescriptionStringPositive;
                            DescriptionValue = 3;
                            valueString = $"{lstValue} {Math.Floor(val1).ToString(CultureInfo.InvariantCulture)}-{Math.Floor(val2 * 99).ToString(CultureInfo.InvariantCulture)} ({valueString} Per Character Level)";
                            break;
                        case 11:
                            valueString = lstValue.Replace("%d", $"{((double)(Utility.ToNullableInt(parameter).Value / 100f)).ToString(CultureInfo.InvariantCulture)}");
                            DescriptionValue = 3;
                            break;
                        case 12:
                            valueString = $"+{valueString}";
                            break;
                        case 13:
                            var classReplace = "";
                            valueString = $"+{valueString}";

                            var regex = Regex.Match(parameter, @"randclassskill(\d+)"); // Work with custom randclasskill(digit) 
                            if (regex.Success)
                            {
                                classReplace = "(Random Class)";
                                valueString = $"+{regex.Groups[1].Value}";
                            }
                            else if (parameter == "randclassskill")
                            {
                                classReplace = "(Random Class)";
                                valueString = "+3";
                            }
                            else
                            {
                                if (!importer.CharStats.TryGetValue(parameter, out charStat))
                                {
                                    throw ItemStatCostException.Create($"Could not find character class '{parameter}'\nNote: if you have made a custom version of 'randclassskill' to support different amount of skills change them to 'randclassskill<d>' for example 'randclassskill5' is supported.");
                                }
                                classReplace = charStat.Class;
                            }
                            lstValue = lstValue.Replace("%d", classReplace);
                            break;
                        case 14:
                            var par = Utility.ToNullableInt(parameter);
                            if (!par.HasValue)
                            {
                                throw ItemStatCostException.Create($"Could not convert parameter '{parameter}' to a valid integer");
                            }

                            if (!importer.SkillTabs.TryGetValue(par.Value, out string? skillTab))
                            {
                                throw ItemStatCostException.Create($"Could not find skill tab with id {par.Value}");
                            }

                            var className = importer.CharStats.Values.First(x => x.StrSkillTab1 == skillTab || x.StrSkillTab2 == skillTab || x.StrSkillTab3 == skillTab).Class;

                            if (!importer.Table.TryGetValue(skillTab, out lstValue))
                            {
                                throw ItemStatCostException.Create($"Could not find translation key '{skillTab}' in any .tbl file");
                            }

                            valueString = $"{lstValue.Replace("%d", valueString)} ({className} only)";
                            break;
                        case 15:
                            valueString = value2.Value.ToString();
                            var skill = importer.GetSkill(parameter);

                            if (value2.Value == 0)
                            {
                                val1 = Math.Min(Math.Ceiling((itemLevel - (skill.RequiredLevel - 1)) / 3.9), 20);
                                val2 = Math.Min(Math.Round((99 - (skill.RequiredLevel - 1)) / 3.9), 20);

                                valueString = GetValueString(val1, val2);
                            }

                            valueString = lstValue.Replace("%d%", value.Value.ToString())
                                                  .Replace("%d", valueString)
                                                  .Replace("%s", skill.SkillDesc);

                            if (string.IsNullOrEmpty(skill.SkillDesc))
                            {
                                throw ItemStatCostException.Create($"Skill for property has missing 'skilldesc' in Skills.txt: name: '{skill.Name}', id: '{skill.Id}'");
                            }

                            break;
                        case 16:
                            valueString = lstValue.Replace("%d", valueString)
                                                  .Replace("%s", importer.GetSkill(parameter).Name);
                            DescriptionValue = 3;
                            break;
                        case 20:
                            valueString = $"{GetValueString(value * -1, value2 * -1)}%";
                            break;
                        case 23:
                            if (!importer.MonStats.TryGetValue(parameter, out MonStat? monstat))
                            {
                                throw ItemStatCostException.Create($"Could not find monster with id '{parameter}' in MonStats.txt");
                            }
                            valueString = $"{valueString}% {lstValue} {monstat.NameStr}";
                            DescriptionValue = 3;
                            break;
                        case 24:
                            valueString = $"Level {value2} {importer.GetSkill(parameter).Name} ({value} Charges)";
                            break;
                        case 27:
                            var charClass = importer.GetSkill(parameter).CharClass;
                            var reqString = "";
                            var shortClass = Utility.ToEnumString(charClass);
                            // Add requirement if one is there
                            if (!importer.CharStats.TryGetValue(shortClass, out charStat))
                            {
                                throw ItemStatCostException.Create($"Could not find character skill tab '{charClass}' property");
                            }

                            reqString = importer.Table.GetValue(charStat.StrClassOnly);
                            //reqString = $" ({Core.Importer.CharStats.Data[shortClass].Class} Only)";
                            valueString = $"+{valueString} to {importer.GetSkill(parameter).Name}{reqString}";
                            break;
                        case 28:
                            valueString = $"+{valueString} to {importer.GetSkill(parameter).Name}";
                            break;
                        case 29: // Custom for sockets
                            if (string.IsNullOrEmpty(valueString))
                            {
                                valueString = parameter;
                            }

                            valueString = $"{lstValue} ({valueString})";
                            break;
                        case 30: // Custom for elemental damage
                            valueString = lstValue.Replace("%d", valueString).Replace("%s", parameter);
                            break;
                        default:
                            // Not implemented function
                            valueString = UnimplementedFunction(value, value2, parameter, Op, OpParam, DescriptionFunction.Value);
                            DescriptionValue = 3;
                            break;
                    }
                }
            }

            // Remove +- in case it happens
            valueString = valueString.Replace("+-", "-");

            if (DescriptionValue.HasValue && !string.IsNullOrEmpty(lstValue))
            {
                switch (DescriptionValue.Value)
                {
                    case 0:
                        valueString = lstValue;
                        break;
                    case 1:
                        valueString = $"{valueString} {lstValue}";
                        break;
                    case 2:
                        valueString = $"{lstValue} {valueString}";
                        break;
                    case 3:
                        // Used if the value already contain all information
                        break;
                }
            }

            // Trim whitespace and remove trailing newline as we sometimes see those in the properties
            valueString = valueString.Trim().Replace("\\n", "");
            return valueString;
        }

        private static double CalculatePerLevel(string parameter, int? op, int? op_param, string stat)
        {
            var para = Utility.ToNullableInt(parameter);
            if (!para.HasValue)
            {
                throw ItemStatCostException.Create($"Could not calculate per level, as parameter '{parameter}' is not a valid integer");
            }

            var val = para.Value / 8d;
            return val;
        }

        private static string GetValueString(double? value = null, double? value2 = null)
        {
            var valueString = "";

            if (value.HasValue)
            {
                valueString += value.Value.ToString(CultureInfo.InvariantCulture);

                if (value2.HasValue && value.Value != value2.Value)
                {
                    if (value2.Value >= 0)
                    {
                        valueString += $"-{value2.Value.ToString(CultureInfo.InvariantCulture)}";
                    }
                    else
                    {
                        valueString += $" to {value2.Value.ToString(CultureInfo.InvariantCulture)}";
                    }
                }
            }

            return valueString;
        }

        private static string UnimplementedFunction(int? value1, int? value2, string paramter, int? op, int? opParam, int function)
        {
            // Sad face :(
            return $"TODO: Unimplemented function: '{function}' value1: '{(value1.HasValue ? value1.Value.ToString() : "null")}' value2: '{(value2.HasValue ? value2.Value.ToString() : "null")}' parameter: '{paramter}' op: '{(op.HasValue ? op.Value.ToString() : "null")}' op_param: '{(opParam.HasValue ? opParam.Value.ToString() : "null")}'";
        }
    }
}
