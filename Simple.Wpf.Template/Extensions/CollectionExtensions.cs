namespace Simple.Wpf.Template.Extensions
{
    using System.Collections.Generic;

    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
            {
                collection.Add(item);
            }
        }
    }
}