namespace Simple.Wpf.Template.ViewModels
{
    using System.Collections.Generic;

    public interface IDateOfBirthViewModel : ICloseableViewModel
    {
        IEnumerable<int> Days { get; }
        IEnumerable<int> Months { get; }
        IEnumerable<int> Years { get; }
        int? Day { get; set; }
        int? Month { get; set; }
        int? Year { get; set; }
    }
}