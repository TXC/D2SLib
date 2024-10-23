using System;
using System.Collections.Generic;

namespace D2SImporter.Attributes
{
    /// <summary>
    /// Import complex data
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ComplexImportAttribute : Attribute
    {
        public Delegate Predicate { get; private set; }

        /// <summary>
        /// Import complex data
        /// </summary>
        /// <param name="callingType">Method signature (Func<Dictionary<string, string>, (Return Type)></param>
        /// <param name="delegateType">Class to call</param>
        /// <param name="delegateName">Methodname to call</param>
        public ComplexImportAttribute(Type callingType, Type delegateType, string delegateName)
        {
            Predicate = Delegate.CreateDelegate(callingType, delegateType, delegateName);
        }

        /// <summary>
        /// Import complex data, as object
        /// </summary>
        /// <param name="delegateType">Class to call</param>
        /// <param name="delegateName">Methodname to call</param>
        public ComplexImportAttribute(Type delegateType, string delegateName)
        {
            Type callingType = typeof(Func<Dictionary<string, string>, object>);

            Predicate = Delegate.CreateDelegate(
                callingType, delegateType, delegateName);
        }
    }
}
