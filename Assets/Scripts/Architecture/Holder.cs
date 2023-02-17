using UnityEngine;

namespace Architecture
{
    public abstract class Holder<T> : ScriptableObject
    {
        public T Value { get; set; }
    }
}