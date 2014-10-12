using System;
using System.Collections.Concurrent;
using System.Linq;

namespace WorkManager.Tests 
{
    static class DataExtensions
    {
        public static bool ContainsValue(this ConcurrentDictionary<Guid, int> dict, int value)
        {
            return dict.Values.Any(v => v == value);
        }
    }
}
