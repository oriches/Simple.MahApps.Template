namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Globalization;
    using System.Reactive.Linq;
    using Extensions;
    using Models;
    using PropertyChanged;
    using Services;

    public sealed class DiagnosticsViewModel : BaseViewModel, IDiagnosticsViewModel
    {
        internal struct FormattedMemory
        {
            public string ManagedMemory { get; }
            public string TotalMemory { get; }

            public FormattedMemory(string managedMemory, string totalMemory)
            {
                ManagedMemory = managedMemory;
                TotalMemory = totalMemory;
            }
        }

        public DiagnosticsViewModel(IDiagnosticsService diagnosticsService, ISchedulerService schedulerService)
        {
            Cpu = Constants.DefaultCpuString;
            ManagedMemory = Constants.DefaultManagedMemoryString;
            TotalMemory = Constants.DefaultTotalMemoryString;

            diagnosticsService.Cpu
                .Select(FormatCpu)
                .DistinctUntilChanged()
                .ObserveOn(schedulerService.Dispatcher)
                .Subscribe(x => Cpu = x,
                    e =>
                    {
                        Logger.Error(e);
                        Cpu = Constants.DefaultCpuString;
                    })
                .DisposeWith(this);

            diagnosticsService.Memory
                .Select(FormatMemory)
                .DistinctUntilChanged()
                .ObserveOn(schedulerService.Dispatcher)
                .Subscribe(x =>
                {
                    ManagedMemory = x.ManagedMemory;
                    TotalMemory = x.TotalMemory;
                }, e =>
                {
                    Logger.Error(e);
                    ManagedMemory = Constants.DefaultManagedMemoryString;
                    TotalMemory = Constants.DefaultTotalMemoryString;
                })
                .DisposeWith(this);
        }

        public string Cpu { get; set; }
       
        public string ManagedMemory { get; set; }

        public string TotalMemory { get; set; }

        private static string FormatCpu(int cpu)
        {
            return cpu < 10
                ? $"CPU: 0{cpu.ToString(CultureInfo.InvariantCulture)} %"
                : $"CPU: {cpu.ToString(CultureInfo.InvariantCulture)} %";
        }

        private static FormattedMemory FormatMemory(Memory memory)
        {
            var managedMemory = $"Managed Memory: {memory.ManagedAsString()}";
            var totalMemory = $"Total Memory: {memory.WorkingSetPrivateAsString()}";

            return new FormattedMemory(managedMemory, totalMemory);
        }
    }
}
