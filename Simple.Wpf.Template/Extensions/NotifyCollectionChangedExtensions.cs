using System;
using System.Collections.Specialized;
using System.Reactive.Linq;

namespace Simple.Wpf.Template.Extensions
{
    public static class NotifyCollectionChangedExtensions
    {
        public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChanged(
            this INotifyCollectionChanged source) =>
            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    h => source.CollectionChanged += h, h => source.CollectionChanged -= h)
                .Select(x => x.EventArgs);
    }
}