using System;
using System.Collections.Generic;

namespace Werehorse.Runtime.Utility.Extensions {
    public static class CollectionExtensions {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action) {
            foreach (T item in collection) {
                action(item);
            }
        }
    }
}
