using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Helpers;
using Simple.Wpf.Template.Models;

namespace Simple.Wpf.Template.ViewModels
{
    public abstract class BaseViewModel : DisposableObject, IViewModel
    {
        private static readonly PropertyChangedEventArgs EmptyChangeArgs = new PropertyChangedEventArgs(string.Empty);

        private static readonly IDictionary<string, PropertyChangedEventArgs> ChangedProperties =
            new Dictionary<string, PropertyChangedEventArgs>();

        private SuspendedNotifications _suspendedNotifications;

        public event PropertyChangedEventHandler PropertyChanged;

        public IDisposable SuspendNotifications()
        {
            if (_suspendedNotifications == null) _suspendedNotifications = new SuspendedNotifications(this);

            return _suspendedNotifications.AddRef();
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression) =>
            OnPropertyChanged(ExpressionHelper.Name(expression));

        protected virtual void OnPropertyChanged() => OnPropertyChanged(null);

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (_suspendedNotifications != null)
            {
                _suspendedNotifications.Add(propertyName);
            }
            else
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    if (propertyName == null)
                    {
                        handler(this, EmptyChangeArgs);
                    }
                    else
                    {
                        PropertyChangedEventArgs args;
                        if (!ChangedProperties.TryGetValue(propertyName, out args))
                        {
                            args = new PropertyChangedEventArgs(propertyName);
                            ChangedProperties.Add(propertyName, args);
                        }

                        handler(this, args);
                    }
                }
            }
        }

        protected virtual bool SetPropertyAndNotify<T>(ref T existingValue, T newValue, Expression<Func<T>> expression)
        {
            if (EqualityComparer<T>.Default.Equals(existingValue, newValue)) return false;

            existingValue = newValue;
            OnPropertyChanged(expression);

            return true;
        }

        private sealed class SuspendedNotifications : IDisposable
        {
            private readonly HashSet<string> _properties = new HashSet<string>();
            private readonly BaseViewModel _target;
            private int _refCount;

            public SuspendedNotifications(BaseViewModel target) => _target = target;

            public void Dispose()
            {
                _target._suspendedNotifications = null;
                _properties.ForEach(x => _target.OnPropertyChanged(x));
            }

            public void Add(string propertyName) => _properties.Add(propertyName);

            public IDisposable AddRef()
            {
                ++_refCount;
                return Disposable.Create(() =>
                {
                    if (--_refCount == 0) Dispose();
                });
            }
        }
    }
}