namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using ViewModels;

    [TestFixture]
    public sealed class DateOfBirthViewModelFixtures : BaseViewModelFixtures
    {
        [Test]
        public void can_not_save_when_dob_is_not_specified()
        {
            // ARRANGE
            // ACT
            var viewModel = new DateOfBirthViewModel();
            
            // ASSERT
            Assert.That(viewModel.ConfirmCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void can_save_when_dob_specified()
        {
            // ARRANGE
            var viewModel = new DateOfBirthViewModel();

            // ACT
            viewModel.Day = viewModel.Days.First();
            viewModel.Month = viewModel.Months.First();
            viewModel.Year = viewModel.Years.First();

            // ASSERT
            Assert.That(viewModel.ConfirmCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void busy_gestures_when_saving()
        {
            // ARRANGE
            var viewModel = new DateOfBirthViewModel();
            viewModel.Day = viewModel.Days.First();
            viewModel.Month = viewModel.Months.First();
            viewModel.Year = viewModel.Years.First();

            // ACT
            viewModel.ConfirmCommand.Execute(null);

            // ASSERT
            GestureService.VerifyAll();
        }

        [Test]
        public void close_requested_when_saved()
        {
            // ARRANGE
            var viewModel = new DateOfBirthViewModel();
            viewModel.Day = viewModel.Days.First();
            viewModel.Month = viewModel.Months.First();
            viewModel.Year = viewModel.Years.First();

            var closeRequested = false;
            viewModel.Closed.Subscribe(x => closeRequested = true);

            // ACT
            viewModel.ConfirmCommand.Execute(null);

            // ASSERT
            Assert.That(closeRequested, Is.True);
        }
    }
}