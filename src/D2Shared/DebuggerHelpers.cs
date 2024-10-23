using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace D2Shared
{
    /// <summary>
    /// 
    /// </summary>
    public static class DebuggerHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="value1"></param>
        /// <param name="includeNullValues"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetDebugText(string key1, object? value1, bool includeNullValues = true, string? prefix = null)
        {
            return GetDebugText([Create(key1, value1)], includeNullValues, prefix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="value1"></param>
        /// <param name="key2"></param>
        /// <param name="value2"></param>
        /// <param name="includeNullValues"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetDebugText(string key1, object? value1, string key2, object? value2, bool includeNullValues = true, string? prefix = null)
        {
            return GetDebugText([Create(key1, value1), Create(key2, value2)], includeNullValues, prefix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key1"></param>
        /// <param name="value1"></param>
        /// <param name="key2"></param>
        /// <param name="value2"></param>
        /// <param name="key3"></param>
        /// <param name="value3"></param>
        /// <param name="includeNullValues"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetDebugText(string key1, object? value1, string key2, object? value2, string key3, object? value3, bool includeNullValues = true, string? prefix = null)
        {
            return GetDebugText([Create(key1, value1), Create(key2, value2), Create(key3, value3)], includeNullValues, prefix);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="includeNullValues"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetDebugText(ReadOnlySpan<KeyValuePair<string, object?>> values, bool includeNullValues = true, string? prefix = null)
        {
            if (values.Length == 0)
            {
                return prefix ?? string.Empty;
            }

            var sb = new StringBuilder();
            if (prefix != null)
            {
                sb.Append(prefix);
            }

            var first = true;
            for (var i = 0; i < values.Length; i++)
            {
                var kvp = values[i];

                if (HasValue(kvp.Value) || includeNullValues)
                {
                    if (first)
                    {
                        if (prefix != null)
                        {
                            sb.Append(' ');
                        }

                        first = false;
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    sb.Append(kvp.Key);
                    sb.Append(": ");
                    if (kvp.Value is null)
                    {
                        sb.Append("(null)");
                    }
                    else if (kvp.Value is string s)
                    {
                        sb.Append(s);
                    }
                    else if (kvp.Value is IEnumerable enumerable)
                    {
                        var firstItem = true;
                        foreach (var item in enumerable)
                        {
                            if (firstItem)
                            {
                                firstItem = false;
                            }
                            else
                            {
                                sb.Append(',');
                            }
                            sb.Append(item);
                        }
                    }
                    else
                    {
                        sb.Append(kvp.Value);
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool HasValue(object? value)
        {
            if (value is null)
            {
                return false;
            }

            // Empty collections don't have a value.
            if (value is not string && value is IEnumerable enumerable && !enumerable.GetEnumerator().MoveNext())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static KeyValuePair<string, object?> Create(string key, object? value)
            => new(key, value);
    }

}
