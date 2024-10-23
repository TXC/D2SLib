using D2SImporter.Attributes;
using D2SImporter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using D2Shared;

namespace D2SImporter
{
    public class BaseImporter
    {
        public Dictionary<int, Unique> Uniques { get; set; }
        public Dictionary<int, Runeword> Runewords { get; set; }
        public Dictionary<string, CubeRecipe> CubeRecipes { get; set; }
        public Dictionary<string, Set> Sets { get; set; }

        public Table Table { get; set; }
        public Dictionary<int, MagicSuffix> MagicSuffixes { get; set; }
        public Dictionary<int, MagicPrefix> MagicPrefixes { get; set; }
        public Dictionary<int, MagicAffix> MagicAffixes { get; set; }
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

        /// <summary>
        /// Because the skill tabs doesn't match the .lst file..
        /// </summary>
        public Dictionary<int, string> SkillTabs { get; set; } = new()
            {
                {0, "StrSklTabItem3" },
                {1, "StrSklTabItem2" },
                {2, "StrSklTabItem1" },
                {3, "StrSklTabItem15" },
                {4, "StrSklTabItem14" },
                {5, "StrSklTabItem13" },
                {6, "StrSklTabItem9" },
                {7, "StrSklTabItem8" },
                {8, "StrSklTabItem7" },
                {9, "StrSklTabItem6" },
                {10, "StrSklTabItem5" },
                {11, "StrSklTabItem4" },
                {12, "StrSklTabItem12" },
                {13, "StrSklTabItem11" },
                {14, "StrSklTabItem10" },
                {15, "StrSklTabItem16" },
                {16, "StrSklTabItem17" },
                {17, "StrSklTabItem18" },
                {18, "StrSklTabItem19" },
                {19, "StrSklTabItem20" },
                {20, "StrSklTabItem21" }
            };

        public BaseImporter()
        {
            ExceptionHandler.Initialize();

            Uniques = [];
            Runewords = [];
            CubeRecipes = [];
            Sets = [];

            Table = new();

            MagicSuffixes = [];
            MagicPrefixes = [];
            MagicAffixes = [];
            ItemStatCosts = [];
            EffectProperties = [];
            ItemTypes = [];
            Armors = [];
            Weapons = [];
            Skills = [];
            RarePrefixes = [];
            RareSuffixes = [];
            CharStats = [];
            MonStats = [];
            Miscs = [];
            Gems = [];
            SetItems = [];
        }

        public virtual void Export()
        {
            throw new NotImplementedException();
        }

        public static Dictionary<string, TValue> ImportFromFile<TValue>(IImporter importer, string excelFolder) where TValue : class, new()
            => ImportFromFile<string, TValue>(importer, excelFolder);

        public static Dictionary<TKey, TValue> ImportFromFile<TKey, TValue>(IImporter importer, string excelFolder)
            where TValue : class, new()
        {
            var attr = typeof(TValue).GetCustomAttribute<FileNameAttribute>();
            var data = Utility.ReadTxtFileToDictionaryList(excelFolder + $"/{attr.FileName}");
            return MapData<TKey, TValue>(importer, data);
        }

        public static Dictionary<string, TValue> ImportFromAssembly<TValue>(IImporter importer, string version) where TValue : class, new()
            => ImportFromAssembly<string, TValue>(importer, version);

        public static Dictionary<TKey, TValue> ImportFromAssembly<TKey, TValue>(IImporter importer, string version)
            where TValue : class, new()
        {
            var attr = typeof(TValue).GetCustomAttribute<FileNameAttribute>();
            var name = $"D2SImporter.Resources._{version}.Excel.{attr.FileName}";
            var data = Utility.ReadAssemblyFileToDictionaryList(Assembly.GetExecutingAssembly(), name);
            return MapData<TKey, TValue>(importer, data);
        }

        public static Dictionary<TKey, TValue> MapData<TKey, TValue>(IImporter importer, List<Dictionary<string, string>> tableList)
            where TValue : class, new()
        {
            var data = new Dictionary<TKey, TValue>();
            Type targetType = typeof(TValue);
            List<MappedData> classMap = ReadProperties(targetType);

            ComplexImportAttribute? complexClass;
            PreImportAttribute? preImport;
            PreImportRowAttribute? preImportRow;
            PostImportAttribute? postImport;
            PostImportRowAttribute? postImportRow;

            try
            {
                complexClass = targetType.GetCustomAttribute<ComplexImportAttribute>();
                preImport = targetType.GetCustomAttribute<PreImportAttribute>();
                preImportRow = targetType.GetCustomAttribute<PreImportRowAttribute>();
                postImport = targetType.GetCustomAttribute<PostImportAttribute>();
                postImportRow = targetType.GetCustomAttribute<PostImportRowAttribute>();

                List<object> preImportRowParameters = [];
                List<object> complexClassParameters = [];
                List<object> postImportRowParameters = [];

                if (preImport?.Predicate is not null)
                {
                    List<object> preImportParameters = GetParameters<TKey, TValue>(preImport.Predicate.GetType(), importer, tableList, data);
                    preImport?.Predicate.DynamicInvoke(preImportParameters);
                }

                int index = 0;
                foreach (var row in tableList)
                {
                    index++;
                    TValue instance = new();

                    if (preImportRow?.Predicate is not null)
                    {
                        if (preImportRowParameters.Count == 0)
                        {
                            preImportRowParameters = GetParameters<TKey, TValue>(preImportRow.Predicate.GetType(), instance, importer, tableList, row, data);
                        }
                        preImportRow.Predicate.DynamicInvoke([.. preImportRowParameters]);
                    }

                    if (complexClass is not null)
                    {
                        if (complexClassParameters.Count == 0)
                        {
                            complexClassParameters = GetParameters<TKey, TValue>(complexClass.Predicate.GetType(), instance, importer, tableList, row, data);
                        }
                        complexClass.Predicate.DynamicInvoke([.. complexClassParameters]);
                    }
                    else
                    {
                        MapData<TValue>(instance, importer, classMap, row);
                    }

                    if (postImportRow?.Predicate is not null)
                    {
                        if (postImportRowParameters.Count == 0)
                        {
                            postImportRowParameters = GetParameters<TKey, TValue>(postImportRow.Predicate.GetType(), instance, importer, tableList, row, data);
                        }
                        postImportRow?.Predicate.DynamicInvoke([.. postImportRowParameters]);
                    }

                    TKey keyValue = GetKey<TKey, TValue>(index, classMap, row, instance);
                    //data.Add(keyValue, instance);
                    if (data.TryAdd(keyValue, instance) == false)
                    {
                        throw new Exception($"Duplicate key {keyValue}");
                    }
                }

                if (postImport?.Predicate is not null)
                {
                    List<object> postImportParameters = GetParameters<TKey, TValue>(postImport.Predicate.GetType(), importer, tableList, data);
                    postImport?.Predicate.DynamicInvoke([.. postImportParameters]);
                }

                return data;
            }
            catch (Exception e)
            {
                Exception ex = new($"Error on data mapping. KeyType: {typeof(TKey)}, Type: {typeof(TValue)}", e);
                ExceptionHandler.WriteException(ex);
                throw ex;
            }
        }

        static void MapData<TValue>(TValue instance, IImporter importer, List<MappedData> classMap, Dictionary<string, string> row)
            where TValue : class
        {
            var instanceType = instance.GetType();
            foreach (var prop in classMap)
            {
                var instanceProperties = instanceType.GetProperties();

                var instancePropertyNames = instanceProperties.Where(x => x.Name == prop.PropertyName);
                System.Reflection.PropertyInfo? instanceProperty = null;
                if (instancePropertyNames.Count() > 1)
                {
                    instanceProperty = instancePropertyNames.FirstOrDefault(x => x.DeclaringType == instanceType);
                }
                else
                {
                    instanceProperty = instancePropertyNames.SingleOrDefault();
                }
                //var instanceProperty = instanceProperties.Where(x => x.Name == prop.PropertyName)
                //                                         .First(x => x.DeclaringType == instanceType);
                if (instanceProperty is null || instanceProperty.CanWrite == false)
                {
                    continue;
                }

                if (prop.Complex is not null)
                {
                    List<object> complexParameters = GetParameters<TValue>(prop.Complex.GetType(), instance, importer, row, classMap, prop);
                    instanceProperty.SetValue(instance, prop.Complex.DynamicInvoke([.. complexParameters]));
                    continue;
                }

                string? rowValue = GetColumnValue(prop, row);
                if (rowValue is null)
                {
                    continue;
                }
                Type propType = Nullable.GetUnderlyingType(instanceProperty.PropertyType) ?? instanceProperty.PropertyType;

                if (prop.IsTranslatable && propType == typeof(string))
                {
                    instanceProperty.SetValue(instance, importer.Table.GetValue(rowValue));
                }
                else if (propType == typeof(string))
                {
                    instanceProperty.SetValue(instance, (string)rowValue);
                }
                else if (propType.BaseType == typeof(Enum))
                {
                    instanceProperty.SetValue(instance, Utility.ToEnum(propType, rowValue));
                }
                else if (propType == typeof(int?))
                {
                    instanceProperty.SetValue(instance, Utility.ToNullableInt(rowValue));
                }
                else if (propType == typeof(int))
                {
                    instanceProperty.SetValue(instance, Utility.ToInt(rowValue));
                }
                else if (propType == typeof(bool))
                {
                    instanceProperty.SetValue(instance, Utility.ToBool(rowValue));
                }
                else if (propType.IsAssignableFrom(typeof(ID2Data)))
                {
                    instanceProperty.SetValue(instance, GetDataType(propType, importer, rowValue));
                }
                else if (propType.BaseType.IsAssignableFrom(typeof(ID2Data)))
                {
                    instanceProperty.SetValue(instance, GetDataType(propType, importer, rowValue));
                }
                else
                {
                    throw new Exception($"Unsupported type '{propType}'");
                }
            }
        }

        static TKey GetKey<TKey, TValue>(int index, List<MappedData> classMap, Dictionary<string, string> row, TValue instance)
            where TValue : class
        {
            var keyColumn = classMap.Where(x => x.IsKey == true).SingleOrDefault();
            if (typeof(TKey) == index.GetType() && index is TKey indexKey)
            {
                return indexKey;
            }
            else if (keyColumn.ColumnName is not null)
            {
                if (row.TryGetValue(keyColumn.ColumnName, out string key) && key is TKey keyValue)
                {
                    return keyValue;
                }
                else if (GetColumnValue(keyColumn, row) is TKey rowValue)
                {
                    return rowValue;
                }
                else
                {
                    throw new Exception($"Invalid key type for '{keyColumn.ColumnName}' ({typeof(TKey)}) on entity {typeof(TValue)}");
                }
            }
            else if (keyColumn.PropertyName is not null)
            {
                var instanceProperty = instance.GetType().GetProperty(keyColumn.PropertyName);
                if (instanceProperty.GetValue(instance) is TKey rowValue)
                {
                    return rowValue;
                }
                else
                {
                    throw new Exception($"Invalid key type for '{keyColumn.PropertyName}' ({typeof(TKey)}) on entity {typeof(TValue)}");
                }
            }
            else
            {
                throw new Exception($"Invalid key on entity {typeof(TValue)}");
            }
        }

        static string? GetColumnValue(MappedData map, Dictionary<string, string> row)
        {
            List<string> columns = [map.ColumnName];
            if (map.ColumnName[..1] == "*")
            {
                columns.Add(map.ColumnName[1..]);
            }
            if (map.AlternateColumnNames.Length > 0)
            {
                foreach (var column in map.AlternateColumnNames)
                {
                    columns.Add(column);
                }
            }
            foreach (var column in columns)
            {
                if (row.TryGetValue(column, out string value))
                {
                    return value;
                }
            }
            return null;
        }

        static object? GetDataType(Type propType, IImporter importer, string rowValue)
        {
            if (propType == typeof(Equipment) || propType.BaseType == typeof(Equipment))
            {
                return importer.GetByCode(rowValue);
            }
            else if (propType == typeof(CharStat))
            {
                if (importer.CharStats.TryGetValue(rowValue, out CharStat type))
                {
                    return type;
                }
            }
            else if (propType == typeof(EffectProperty))
            {
                if (importer.EffectProperties.TryGetValue(rowValue, out EffectProperty type))
                {
                    return type;
                }
            }
            else if (propType == typeof(ItemStatCost))
            {
                if (importer.ItemStatCosts.TryGetValue(rowValue, out ItemStatCost type))
                {
                    return type;
                }
            }
            else if (propType == typeof(ItemType))
            {
                if (importer.ItemTypes.TryGetValue(rowValue, out ItemType type))
                {
                    return type;
                }
            }
            else if (propType.BaseType == typeof(MagicAffix))
            {
                if (int.TryParse(rowValue, out int value))
                {
                    return importer.GetMagicAffixById(value);
                }
            }
            else if (propType == typeof(MonStat))
            {
                if (importer.ItemTypes.TryGetValue(rowValue, out ItemType type))
                {
                    return type;
                }
            }
            else if (propType.BaseType == typeof(RareAffix))
            {
                if (int.TryParse(rowValue, out int value))
                {
                    return importer.GetMagicAffixById(value);
                }
            }
            else if (propType == typeof(Skill))
            {
                if (importer.Skills.TryGetValue(rowValue, out Skill type))
                {
                    return type;
                }
            }
            else if (propType == typeof(CubeRecipe))
            {
                if (importer.CubeRecipes.TryGetValue(rowValue, out CubeRecipe type))
                {
                    return type;
                }
            }
            else if (propType == typeof(Set))
            {
                if (importer.Sets.TryGetValue(rowValue, out Set set))
                {
                    return set;
                }
            }
            else if (propType.BaseType == typeof(Item))
            {
                if (importer.SetItems.TryGetValue(rowValue, out SetItem setItem))
                {
                    return setItem;
                }
                else if (importer.Runewords.TryGetValue(Utility.ToInt(rowValue), out Runeword runeword))
                {
                    return runeword;
                }
                else if (importer.Gems.TryGetValue(rowValue, out Gem gem))
                {
                    return gem;
                }
                //else if (importer.Uniques.TryGetValue(rowValue, out Unique unique))
                else if (importer.Uniques.Any(x => x.Value.Index == rowValue))
                {
                    Unique unique = importer.Uniques.First(x => x.Value.Index == rowValue).Value;
                    return unique;
                }
            }
            return null;
        }

        static List<object> GetParameters<TValue>(Type type, params object[] parameters) where TValue : class
            => GetParameters<object, TValue>(type, [.. parameters]);

        static List<object> GetParameters<TKey, TValue>(Type type, params object[] parameters) where TValue : class
        {
            List<object> result = [];

            ParameterInfo[] arguments = type.GetMethod("Invoke").GetParameters();

            foreach (var arg in arguments)
            {
                Type paramType = Nullable.GetUnderlyingType(arg.ParameterType) ?? arg.ParameterType;
                Type paramBType = paramType.BaseType;

                object? param = null;
                /*
                var param = parameters.FirstOrDefault(x => x.GetType() == paramType
                                                        || x.GetType().BaseType == paramType);
                param ??= parameters.FirstOrDefault(x => x.GetType() == paramType.BaseType
                                                        || x.GetType().BaseType == paramType.BaseType);
                param ??= parameters.FirstOrDefault(x => x.GetType() == arg.ParameterType
                                                        || x.GetType().BaseType == arg.ParameterType);
                param ??= parameters.FirstOrDefault(x => x.GetType() == arg.ParameterType.BaseType
                                                        || x.GetType().BaseType == arg.ParameterType.BaseType);
                */
                /*
                if (param is null)
                {
                    */
                foreach (var p in parameters)
                {
                    Type pType = p.GetType();
                    Type pbType = pType.BaseType;

                    if (paramType == pType)
                    {
                        param = p;
                        break;
                    }
                    else if (paramType == pType.BaseType)
                    {
                        param = p;
                        break;
                    }
                    if (paramType.BaseType == pType)
                    {
                        param = p;
                        break;
                    }
                    else if (paramType == typeof(IImporter) && p is IImporter)
                    {
                        param = p;
                        break;
                    }
                }
                /*
            }
            */
                if (param is null)
                {
                    continue;
                }

                result.Add(param);
            }

            return result;
        }

        static List<MappedData> ReadProperties(Type type)
        {
            List<MappedData> data = [];
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                try
                {
                    ColumnNameAttribute? column = prop.GetCustomAttribute<ColumnNameAttribute>();
                    ComplexImportAttribute? node = prop.GetCustomAttribute<ComplexImportAttribute>();
                    KeyAttribute? keycolumn = prop.GetCustomAttribute<KeyAttribute>();

                    if (column is null && node is null && keycolumn is null)
                    {
                        continue;
                    }

                    string? columnName = null;
                    if (prop.CanWrite)
                    {
                        if (!string.IsNullOrEmpty(column?.Column))
                        {
                            columnName = column.Column;
                        }
                        else
                        {
                            columnName = prop.Name;
                        }
                    }
                    var translatable = prop.GetCustomAttribute<TranslatableAttribute>();

                    data.Add(new MappedData()
                    {
                        IsKey = keycolumn?.IsKey ?? false,
                        IsTranslatable = translatable?.IsTranslatable ?? false,
                        ColumnName = columnName,
                        PropertyName = prop.Name,
                        AlternateColumnNames = column?.AlternateColumn ?? [],
                        Complex = node?.Predicate ?? null,
                    });
                }
                catch (Exception e)
                {
                    Exception ex = new($"Error on Property Read. Type: {type.Name} Property: {prop.Name}", e);
                    ExceptionHandler.WriteException(ex);
                    throw ex;
                }
            }
            return data;
        }

        #region Helpers
        public Skill? GetSkill(string skill)
        {
            if (Skills.Values.Where(x => x.Id == Utility.ToNullableInt(skill)).Any())
            {
                return Skills.Values.Where(x => x.Id == Utility.ToNullableInt(skill)).FirstOrDefault();
            }
            else if (Skills.Values.Where(x => x.Name == skill).Any())
            {
                return Skills.Values.Where(x => x.Name == skill).FirstOrDefault();
            }
            else if (Skills.Values.Where(x => x.SkillDesc == skill).Any())
            {
                return Skills.Values.Where(x => x.SkillDesc == skill).FirstOrDefault();
            }
            //throw new Exception(message: $"Could not find skill with id, name, or description '{skill}' in Skills.txt");
            return null;
        }

        public Equipment? GetByCode(string code)
        {
            string trimmedCode = code.TrimEnd();
            if (Armors.TryGetValue(trimmedCode, out Armor armor))
            {
                return armor;
            }
            else if (Weapons.TryGetValue(trimmedCode, out Weapon weapon))
            {
                return weapon;
            }
            else if (Miscs.TryGetValue(trimmedCode, out Misc misc))
            {
                return misc;
            }
            return null;
        }

        public MagicAffix? GetMagicAffixById(int id)
        {
            if (MagicSuffixes.TryGetValue(id, out MagicSuffix suffix))
            {
                return suffix;
            }
            else if (MagicPrefixes.TryGetValue(id, out MagicPrefix prefix))
            {
                return prefix;
            }
            else if (MagicAffixes.TryGetValue(id, out MagicAffix affix))
            {
                return affix;
            }
            return null;
        }

        public RareAffix? GetRareAffixById(int id)
        {
            if (RareSuffixes.TryGetValue(id, out RareSuffix suffix))
            {
                return suffix;
            }
            else if (RarePrefixes.TryGetValue(id, out RarePrefix prefix))
            {
                return prefix;
            }
            //else if (RareAffixes.TryGetValue(id, out RareAffix affix))
            //{
            //    return affix;
            //}
            return null;
        }

        public string ReplaceItemName(string item)
        {
            if (ItemTypes.TryGetValue(item, out ItemType itemType))
            {
                return itemType.Name;
            }
            else if (Miscs.TryGetValue(item, out Misc misc))
            {
                return misc.Name;
            }
            else if (Weapons.TryGetValue(item, out Weapon weapon))
            {
                return weapon.Name;
            }
            else if (Armors.TryGetValue(item, out Armor armor))
            {
                return armor.Name;
            }

            return item;
        }

        public bool IsStackable(string code)
        {
            if (Armors.TryGetValue(code, out Armor armor))
            {
                return armor.Stackable;
            }
            else if (Weapons.TryGetValue(code, out Weapon weapon))
            {
                return weapon.Stackable;
            }
            else if (Miscs.TryGetValue(code, out Misc misc))
            {
                return misc.Stackable;
            }
            return false;
        }

        public bool IsArmor(string code)
            => Armors.ContainsKey(code.TrimEnd());

        public bool IsWeapon(string code)
            => Weapons.ContainsKey(code.TrimEnd());

        public bool IsMisc(string code)
            => Miscs.ContainsKey(code.TrimEnd());
        #endregion Helpers
    }

    internal struct MappedData
    {
        public bool IsKey { get; set; }
        public bool IsTranslatable { get; set; }
        public string? ColumnName { get; set; }
        public string PropertyName { get; set; }
        public string[] AlternateColumnNames { get; set; }
        public Delegate? Complex { get; set; }

        public override string ToString()
        {
            string str = $"Prop: {PropertyName}";

            if (!string.IsNullOrEmpty(ColumnName) && ColumnName != PropertyName)
            {
                str += $", Col: {ColumnName}";
            }
            if (AlternateColumnNames.Length > 0)
            {
                str += $", Alt: {string.Join(", ", AlternateColumnNames)}";
            }
            return $"{str}, IsKey: {IsKey}, IsTranslatable: {IsTranslatable}, Complex: {Complex is not null}";
        }
    }
}
