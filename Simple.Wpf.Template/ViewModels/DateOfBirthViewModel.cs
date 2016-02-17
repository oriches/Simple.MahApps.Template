namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using Extensions;
    using PropertyChanged;

    [ImplementPropertyChanged]
    public sealed class DateOfBirthViewModel : CloseableViewModel, IDateOfBirthViewModel
    {
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

        public int? Day { get; set; }

        public int? Month { get; set; }

        public int? Year { get; set; }

        protected override IObservable<bool> InitialiseCanConfirm()
        {
            return this.ObservePropertyChanged(x => Day, x => Month, x => Year)
               .Select(x => Day.HasValue && Month.HasValue && Year.HasValue)
               .StartWith(false);
        }
    }
}
