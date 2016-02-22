namespace Simple.Wpf.Template.ViewModels
{
    using Commands;

    public interface IMainViewModel : IViewModel
    {
        ReactiveCommand<object> RefreshCommand { get; }
        IDiagnosticsViewModel Diagnostics { get; }
    }
}