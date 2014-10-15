namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reactive.Disposables;
    using System.Windows.Input;
    using Commands;
    using Extensions;
    using NLog;
    using Services;

    public sealed class DateOfBirthViewModel : CloseableViewModel
    {
        private readonly IGestureService _gestureService;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;
        private int? _day;
        private int? _month;
        private int? _year;

        public DateOfBirthViewModel(IGestureService gestureService)
        {
            _gestureService = gestureService;

            Days = Enumerable.Range(1, 31)
                .Select(x => new NameValueViewModel<int>(x.ToString(CultureInfo.InvariantCulture), x))
                .ToObservableCollection();

            Months = Enumerable.Range(1, 12)
                .Select(x => new NameValueViewModel<int>(x.ToString(CultureInfo.InvariantCulture), x))
                .ToObservableCollection();

            Years = Enumerable.Range(DateTime.Now.Year - 120, 121).OrderByDescending(x => x)
                .Select(x => new NameValueViewModel<int>(x.ToString(CultureInfo.InvariantCulture), x))
                .ToObservableCollection();

            SaveCommand = new RelayCommand(Save, CanSave);
            
            _disposable = Disposable.Create(() =>
            {
                SaveCommand = null;
            });
        }

        public override void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                base.Dispose();
                
                _disposable.Dispose();
            }
        }

        public ICommand SaveCommand { get; private set; }

        public IEnumerable<NameValueViewModel<int>> Days { get; private set; }

        public IEnumerable<NameValueViewModel<int>> Months { get; private set; }

        public IEnumerable<NameValueViewModel<int>> Years { get; private set; }

        public int? Day
        {
            get
            {
                return _day;
            }

            set
            {
                SetPropertyAndNotify(ref _day, value, () => Day);
            }
        }

        public int? Month
        {
            get
            {
                return _month;
            }

            set
            {
                SetPropertyAndNotify(ref _month, value, () => Month);
            }
        }

        public int? Year
        {
            get
            {
                return _year;
            }

            set
            {
                SetPropertyAndNotify(ref _year, value, () => Year);
            }
        }

        private bool CanSave()
        {
            return _day.HasValue && _month.HasValue && _year.HasValue;
        }

        private void Save()
        {
            _gestureService.SetBusy();

            // Shouldn't really block the UI thread, just here to demo the gesture service...
            System.Threading.Thread.Sleep(3123);

            Close();
        }
    }
}
