using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.StableRef
{
    [Serializable]
    public abstract class StableRefBase
    {
        [SerializeField] public string TypeId;
        [SerializeField] public string TypeDisplayName;
        [SerializeField] public List<UnityEngine.Object> ObjectRefs = new();
        [SerializeField] public List<string> ObjectRefPaths = new();
        [SerializeField] public string ValuesData;
    }
    
    [Serializable]
    public sealed class StableRef<T> : StableRefBase where T : class
    {
        [SerializeReference, StableRefSelector] public T Value;
    }
}