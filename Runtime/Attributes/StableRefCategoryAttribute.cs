using System;

namespace SST.StableRef
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class StableRefCategoryAttribute : Attribute
    {
        public string Category { get; }
        public StableRefCategoryAttribute(string category) => Category = category;
    }
}