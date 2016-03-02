namespace Simple.Wpf.Template
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using Autofac;
    using Autofac.Core;
    using Rest;
    using ViewModels;

    public static class BootStrapper
    {
        private static ILifetimeScope _rootScope;
        private static IChromeViewModel _chromeViewModel;

        public static IViewModel RootVisual
        {
            get
            {
                if (_rootScope == null)
                {
                    Start();
                }

                _chromeViewModel = _rootScope.Resolve<IChromeViewModel>();
                return _chromeViewModel;
            }
        }

        public static void Start()
        {
            if (_rootScope != null)
            {
                return;
            }

            var builder = new ContainerBuilder();
            var assembly = Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Service"))
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterType<RestClient>()
                .SingleInstance()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .Where(t => !t.IsAssignableFrom(typeof(ITransientViewModel)))
                .AsImplementedInterfaces();

            // several view model instances are transitory and created on the fly, if these are tracked by the container then they
            // won't be disposed of in a timely manner

            var transientType = typeof (ITransientViewModel);

            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("ViewModel"))
                .Where(t =>
                       {
                           var isAssignable = transientType.IsAssignableFrom(t);
                           if (isAssignable)
                           {
                               Debug.WriteLine("Transient view model - " + t.Name);
                           }

                           return isAssignable;
                       })
                .AsImplementedInterfaces()
                .ExternallyOwned();

            _rootScope = builder.Build();
        }

        public static void Stop()
        {
            _rootScope.Dispose();
        }

        public static T Resolve<T>()
        {
            if (_rootScope == null)
            {
                throw new Exception("Bootstrapper hasn't been started!");
            }

            return _rootScope.Resolve<T>(new Parameter[0]);
        }

        public static T Resolve<T>(Parameter[] parameters)
        {
            if (_rootScope == null)
            {
                throw new Exception("Bootstrapper hasn't been started!");
            }

            return _rootScope.Resolve<T>(parameters);
        }
    }
}