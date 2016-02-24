namespace Simple.Wpf.Template.ViewModels
{
    public interface IModifyResourceViewModel : ICloseableViewModel
    {
        string Path { get; }

        string Json { get; set; }
    }
}