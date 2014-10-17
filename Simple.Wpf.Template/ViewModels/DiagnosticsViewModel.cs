namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Extensions;
    using Models;
    using NLog;
    using Services;

    public sealed class DiagnosticsViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CompositeDisposable _disposable;
        private readonly ObservableCollection<string> _log;

        private string _cpu;
        private string _managedMemory;
        private string _totalMemory;
        private string _fps;
        
        internal sealed class FormattedMemory
        {
            public string ManagedMemory { get; private set; }
            public string TotalMemory { get; private set; }

            public FormattedMemory(string managedMemory, string totalMemory)
            {
                ManagedMemory = managedMemory;
                TotalMemory = totalMemory;
            }
        }

        public DiagnosticsViewModel(IDiagnosticsService diagnosticsService, ISchedulerService schedulerService)
        {
            
            Id = string.Format("Identifier: {0}", Guid.NewGuid());

            Fps = Constants.DefaultFpsString;
            Cpu = Constants.DefaultCpuString;
            ManagedMemory = Constants.DefaultManagedMemoryString;
            TotalMemory = Constants.DefaultTotalMemoryString;

            _log = new ObservableCollection<string>();
            
            var target = (LimitedMemoryTarget) LogManager.Configuration.FindTargetByName("memory");
            _disposable = new CompositeDisposable
            {
                Observable.Interval(Constants.DiagnosticsLogInterval, schedulerService.TaskPool)
                    .Synchronize()
                    .Select(x => target.Logs.Except(_log).ToArray())
                    .Where(x => x.Any())
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x => _log.AddRange(x)),

                diagnosticsService.Fps
                    .DistinctUntilChanged()
                    .Select(FormatFps)
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x => { Fps = x; },
                               e => { Fps = Constants.DefaultFpsString; }),

                diagnosticsService.Cpu
                    .Select(FormatCpu)
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x => { Cpu = x; },
                               e => { Cpu = Constants.DefaultCpuString; }),

                diagnosticsService.Memory
                    .Select(FormatMemory)
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x =>
                    {
                        ManagedMemory = x.ManagedMemory;
                        TotalMemory = x.TotalMemory;
                    }, e =>
                    {
                        ManagedMemory = Constants.DefaultManagedMemoryString;
                        TotalMemory = Constants.DefaultTotalMemoryString;
                    })
            };
        }
        
        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public string Id { get; private set; }

        public IEnumerable<string> Log { get { return _log; } }

        public string Fps
        {
            get
            {
                return _fps;
            }

            private set
            {
                SetPropertyAndNotify(ref _fps, value, () => Fps);
            }
        }

        public string Cpu
        {
            get
            {
                return _cpu;
            }

            private set
            {
                SetPropertyAndNotify(ref _cpu, value, () => Cpu);
            }
        }

        public string ManagedMemory
        {
            get
            {
                return _managedMemory;
            }

            private set
            {
                SetPropertyAndNotify(ref _managedMemory, value, () => ManagedMemory);
            }
        }

        public string TotalMemory
        {
            get
            {
                return _totalMemory;
            }

            private set
            {
                SetPropertyAndNotify(ref _totalMemory, value, () => TotalMemory);
            }
        }

        private static string FormatFps(int fps)
        {
            return "Render: " + fps.ToString(CultureInfo.InvariantCulture) + " FPS";
        }

        private static string FormatCpu(int cpu)
        {
            return cpu < 10
                ? string.Format("CPU: 0{0} %", cpu.ToString(CultureInfo.InvariantCulture))
                : string.Format("CPU: {0} %", cpu.ToString(CultureInfo.InvariantCulture));
        }

        private static FormattedMemory FormatMemory(Memory memory)
        {
            var managedMemory = string.Format("Managed Memory: {0}", memory.ManagedAsString());
            var totalMemory = string.Format("Total Memory: {0}", memory.WorkingSetPrivateAsString());

            return new FormattedMemory(managedMemory, totalMemory);
        }
    }
}