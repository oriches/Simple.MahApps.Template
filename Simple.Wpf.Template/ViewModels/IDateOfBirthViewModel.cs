using System.Collections.Generic;

namespace Simple.Wpf.Template.ViewModels
{
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