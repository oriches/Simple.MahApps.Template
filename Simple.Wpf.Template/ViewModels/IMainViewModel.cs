namespace Simple.Wpf.Template.ViewModels
{
    using Commands;

    public interface IMainViewModel : IViewModel
    {
        ReactiveCommand<object> DobCommand { get; }
        IDiagnosticsViewModel Diagnostics { get; }
    }
}