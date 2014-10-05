using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZipperUpper.Models
{
    public sealed class ItemCache : Queue<KeyValuePair<String, List<String>>>
    {
        private static readonly Lazy<ItemCache> lazy =
            new Lazy<ItemCache>(() => new ItemCache());

        public static ItemCache Instance { get { return lazy.Value; } }

        private ItemCache()
        {}

        public void AddDataToDictionary(String key, List<String> value)
        {
            if (Count > 10000)
            {
                Dequeue();
            }

            Enqueue(new KeyValuePair<String, List<String>>(key, value));
        }

        //public List<String> this[String key]
        //{
            
        //}
    }
}