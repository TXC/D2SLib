using System;

namespace D2SImporter.Attributes
{
    /// <summary>
    /// A value indicating whether comments are allowed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class FileNameAttribute : Attribute
    {
        /// <summary>
        /// Gets a value indicating whether comments are allowed.
        /// </summary>
        public string FileName { get; private set; }

        public FileNameAttribute(string fileName)
        {
            FileName = fileName;
        }
    }
}
