namespace Simple.Wpf.Template.ViewModels
{
    using System.Collections.Generic;

    public interface IDiagnosticsViewModel : IViewModel
    {
        IEnumerable<string> Log { get; }
        string Cpu { get; }
        string ManagedMemory { get; }
        string TotalMemory { get; }
    }
}