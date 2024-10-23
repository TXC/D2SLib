using System;

namespace D2SImporter.Attributes
{
    /// <summary>
    /// A value indicating whether comments are allowed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TranslatableAttribute : Attribute
    {
        public bool IsTranslatable { get; private set; }

        public TranslatableAttribute()
        {
            IsTranslatable = true;
        }

        public TranslatableAttribute(bool IsTranslatable)
        {
            this.IsTranslatable = IsTranslatable;
        }
    }
}
