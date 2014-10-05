using System;
using System.Collections.Generic;

namespace ZipperUpper.Models
{
    /// <summary> 
    /// Persistent Dictionary that uses a Queue to ensure Dictionary doesn't become bloated.
    /// </summary> 
    /// <remarks> 
    /// Limit is 1000 items 
    /// </remarks> 
    public class CacheDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _dictionary;
        private readonly Queue<TKey> _keys;
        private readonly int _capacity;

        private static readonly Lazy<CacheDictionary<String, List<String>>> lazy =
            new Lazy<CacheDictionary<String, List<String>>>(() => new CacheDictionary<String, List<String>>(1000));

        public static CacheDictionary<String, List<String>> Instance { get { return lazy.Value; } }

        private CacheDictionary(int capacity)
        {
            _keys = new Queue<TKey>(capacity);
            _capacity = capacity;
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public void Add(TKey key, TValue value)
        {
            if (_dictionary.Count == _capacity)
            {
                var oldestKey = _keys.Dequeue();
                _dictionary.Remove(oldestKey);
            }

            _dictionary.Add(key, value);
            _keys.Enqueue(key);
        }

        public TValue this[TKey key]
        {
            get { return _dictionary[key]; }
        }
    }
}