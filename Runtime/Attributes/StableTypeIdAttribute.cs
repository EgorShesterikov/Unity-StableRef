using System;

namespace Utility.StableRef
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public sealed class StableTypeIdAttribute : Attribute
    {
        public string Id { get; }
        public StableTypeIdAttribute(string id) => Id = id;
    }
}