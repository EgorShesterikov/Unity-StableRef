using System;

namespace Utility.StableRef
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class StableRefCategoryAttribute : Attribute
    {
        public string Category { get; }
        public StableRefCategoryAttribute(string category) => Category = category;
    }
}