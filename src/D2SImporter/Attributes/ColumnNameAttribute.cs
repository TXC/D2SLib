using System;

namespace D2SImporter.Attributes
{
    /// <summary>
    /// A value indicating whether comments are allowed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnNameAttribute : Attribute
    {
        /// <summary>
        /// If value begins with <b>*</b> then search both with and without that character
        /// </summary>
        public string? Column { get; private set; }

        /// <summary>
        /// If value begins with <b>*</b> then search both with and without that character
        /// </summary>
        public string[] AlternateColumn { get; private set; } = [];

        public ColumnNameAttribute()
        {
        }

        public ColumnNameAttribute(string column)
        {
            Column = column;
        }
        public ColumnNameAttribute(string column, params string[] alternate)
        {
            Column = column;
            AlternateColumn = alternate;
        }
    }
}
