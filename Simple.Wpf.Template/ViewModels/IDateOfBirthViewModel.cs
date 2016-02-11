namespace Simple.Wpf.Template.ViewModels
{
    using System.Collections.Generic;
    using System.Windows.Input;

    public interface IDateOfBirthViewModel : ICloseableViewModel
    {
        ICommand SaveCommand { get; }
        IEnumerable<int> Days { get; }
        IEnumerable<int> Months { get; }
        IEnumerable<int> Years { get; }
        int? Day { get; set; }
        int? Month { get; set; }
        int? Year { get; set; }
    }
}