namespace Simple.Wpf.Template.ViewModels
{
    using System.Collections.Generic;

    public interface IDiagnosticsViewModel : IViewModel
    {
        string Id { get; }
        IEnumerable<string> Log { get; }
        string Cpu { get; }
        string ManagedMemory { get; }
        string TotalMemory { get; }
    }
}