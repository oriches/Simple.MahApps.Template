namespace Simple.Wpf.Template.ViewModels
{
    using System.Windows.Input;

    public interface IChromeViewModel : IViewModel
    {
        IMainViewModel Main { get; }
        ICommand CloseOverlayCommand { get; }
        bool HasOverlay { get; }
        string OverlayHeader { get; }
        BaseViewModel Overlay { get; }
    }
}