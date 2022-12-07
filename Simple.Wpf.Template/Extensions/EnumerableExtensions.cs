using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Simple.Wpf.Template.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable) action(item);

            return enumerable;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable) =>
            new ObservableCollection<T>(enumerable);
    }
}