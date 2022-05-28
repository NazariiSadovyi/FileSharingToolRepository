using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace QRSharingApp.ViewModel.Helpers
{
    public static class ReactiveExtension
    {
        public static IObservable<Unit> IgnoreValue<T>(this IObservable<T> source)
        {
            return source.Select(_ => Unit.Default);
        }
    }
}
