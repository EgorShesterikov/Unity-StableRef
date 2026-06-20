using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.StableRef
{
    [Serializable]
    public abstract class StableRefListBase { }

    [Serializable]
    public sealed class StableRefList<T> : StableRefListBase, IEnumerable<StableRef<T>> where T : class
    {
        [SerializeField] private List<StableRef<T>> _items = new();

        public int Count => _items?.Count ?? 0;

        public StableRef<T> this[int index] => _items[index];

        public List<StableRef<T>>.Enumerator GetEnumerator()
            => (_items ??= new List<StableRef<T>>()).GetEnumerator();

        IEnumerator<StableRef<T>> IEnumerable<StableRef<T>>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}