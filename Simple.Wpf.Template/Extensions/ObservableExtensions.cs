namespace Simple.Wpf.Template.Extensions
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;

    public static class ObservableExtensions
    {
        public static IObservable<Unit> AsUnit<T>(this IObservable<T> observable)
        {
            return observable.Select(x => Unit.Default);
        }
    }
}
