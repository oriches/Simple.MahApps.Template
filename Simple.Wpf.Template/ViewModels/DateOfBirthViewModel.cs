namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Extensions;

    public sealed class DateOfBirthViewModel : CloseableViewModel, IDateOfBirthViewModel
    {
        private int? _day;
        private int? _month;
        private int? _year;

        public DateOfBirthViewModel()
        {
            Days = Enumerable.Range(1, 31)
                .ToArray();

            Months = Enumerable.Range(1, 12)
                .ToArray();

            Years = Enumerable.Range(DateTime.Now.Year - 120, 121)
                .OrderByDescending(x => x)
                .ToArray();
        }

        public IEnumerable<int> Days { get; }

        public IEnumerable<int> Months { get; }

        public IEnumerable<int> Years { get; }

        public int? Day
        {
            get { return _day; }
            set { SetPropertyAndNotify(ref _day, value, () => Day); }
        }

        public int? Month
        {
            get { return _month; }
            set { SetPropertyAndNotify(ref _month, value, () => Month); }
        }

        public int? Year
        {
            get { return _year; }
            set { SetPropertyAndNotify(ref _year, value, () => Year); }
        }

        protected override IObservable<bool> InitialiseCanConfirm()
        {
            return this.ObservePropertyChanged(x => Day, x => Month, x => Year)
               .Select(x => Day.HasValue && Month.HasValue && Year.HasValue)
               .StartWith(false);
        }
    }
}
