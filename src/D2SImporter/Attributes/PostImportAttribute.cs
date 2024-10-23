using System;

namespace D2SImporter.Attributes
{
    /// <summary>
    /// Import complex data
    /// </summary>
    /// <param name="callingType">Method signature (<c>Action<Dictionary<TKey, TValue>></c></param>
    /// <param name="delegateType">Class to call</param>
    /// <param name="delegateName">Methodname to call</param>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class PostImportAttribute(Type callingType, Type delegateType, string delegateName) : Attribute
    {
        public Delegate Predicate { get; private set; } = Delegate.CreateDelegate(callingType, delegateType, delegateName);
    }
}
