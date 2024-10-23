using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace D2Shared
{
    public static class Utility
    {
        public static int? ToNullableInt(string s)
        {
            if (int.TryParse(s, out int i))
            {
                return i;
            }
            return null;
        }

        public static int ToInt(string s)
        {
            if (int.TryParse(s, out int i))
            {
                return i;
            }
            return 0;
        }

        public static bool ToBool(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }
            return s == "1";
        }

        public static string ToEnumString<T>(T type)
            where T : Enum
        {
            var enumType = typeof(T);
            var name = Enum.GetName(enumType, type);
            var attributes = (EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true);
            if (attributes.Length == 0)
            {
                return name;
            }
            /*
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType
                                                        .GetField(name)
                                                        .GetCustomAttributes(typeof(EnumMemberAttribute), true))
                                                        .SingleOrDefault();
            */
            var enumMemberAttribute = attributes.SingleOrDefault();
            if (enumMemberAttribute is not null)
            {
                return enumMemberAttribute.Value;
            }
            return name;
        }

        public static T? ToEnum<T>(string str)
            where T : Enum
        {
            if (Enum.TryParse(typeof(T), str, true, out object result))
            {
                return (T?)result;
            }

            var enumType = typeof(T);
            foreach (var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).SingleOrDefault();
                if (enumMemberAttribute is null)
                {
                    continue;
                }
                if (enumMemberAttribute.Value == str)
                {
                    return (T)Enum.Parse(enumType, name);
                }
            }
            //throw exception or whatever handling you want or
            return default;
        }

        public static Enum? ToEnum(Type type, string str)
        {
            if (Enum.TryParse(type, str, true, out object result))
            {
                return (Enum)result;
            }

            foreach (var name in Enum.GetNames(type))
            {
                var enumMemberAttribute = ((EnumMemberAttribute[])type.GetField(name).GetCustomAttributes(typeof(EnumMemberAttribute), true)).SingleOrDefault();
                if (enumMemberAttribute is null)
                {
                    continue;
                }
                if (enumMemberAttribute.Value == str)
                {
                    return (Enum)Enum.Parse(type, name);
                }
            }
            //throw exception or whatever handling you want or
            return default;
        }

        public static List<string> ReadTxtFileToList(string path)
        {
            return [.. File.ReadAllLines(path)];
        }

        public static List<string> ReadAssemblyFileToList(string path)
        {
            return ReadLines(() => Assembly.GetExecutingAssembly()
                                    .GetManifestResourceStream(path))
                                    .ToList();
        }

        public static List<Dictionary<string, string>> ReadTxtFileToDictionaryList(string path)
        {
            if (File.Exists(path) == false)
            {
                return [];
            }
            var fileArray = File.ReadAllLines(path);
            return ParseTextToDictionaryList(fileArray);
        }

        public static List<Dictionary<string, string>> ReadAssemblyFileToDictionaryList(Assembly assembly, string name)
        {
            var fileArray = ReadLines(() => assembly.GetManifestResourceStream(name)).ToArray();
            return ParseTextToDictionaryList(fileArray);
        }

        public static List<Dictionary<string, string>> ParseTextToDictionaryList(string[] contents)
        {
            var table = new List<Dictionary<string, string>>();

            var headerArray = contents.Take(1).First().Split('\t');

            var header = new List<string>();

            foreach (var column in headerArray)
            {
                // filter out comment indicator
                string col = column.Trim();
                //if (col.StartsWith('*'))
                //{
                //    col = col[1..];
                //}
                header.Add(col);
            }

            var dataArray = contents.Skip(1);
            foreach (var valueLine in dataArray)
            {
                var values = valueLine.Split('\t');
                if (string.IsNullOrEmpty(values[1]))
                {
                    continue;
                }

                var row = new Dictionary<string, string>();

                for (var i = 0; i < values.Length; i++)
                {
                    row[header[i]] = values[i];
                }

                table.Add(row);
            }

            return table;
        }

        private static IEnumerable<string> ReadLines(Func<Stream> streamProvider)
            => ReadLines(streamProvider, Encoding.UTF8);

        private static IEnumerable<string> ReadLines(Func<Stream> streamProvider, Encoding encoding)
        {
            using Stream stream = streamProvider();
            if (stream == null)
            {
                yield return string.Empty;
                yield break;
            }
            using StreamReader reader = new(stream, encoding);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}
