using System;

namespace D2SImporter.Attributes
{
    /// <summary>
    /// A value indicating whether comments are allowed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeyAttribute : Attribute
    {
        public bool IsKey { get; private set; }

        public KeyAttribute()
        {
            IsKey = true;
        }

        public KeyAttribute(bool IsKey)
        {
            this.IsKey = IsKey;
        }
    }
}
