namespace Simple.Wpf.Template.Extensions
{
    using System;
    using Services;
    using ViewModels;

    public static class BaseViewModelExtensions
    {
        public static T DisposeWith<T>(this T instance, BaseViewModel viewModel) where T : IDisposable
        {
            viewModel.Add(instance);

            return instance;
        }

        public static T DisposeWith<T>(this T instance, BaseService service) where T : IDisposable
        {
            service.Add(instance);

            return instance;
        }
    }
}