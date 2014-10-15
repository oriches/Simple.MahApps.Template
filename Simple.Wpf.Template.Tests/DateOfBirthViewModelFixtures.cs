namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Extensions;
    using Moq;
    using NUnit.Framework;
    using Services;
    using ViewModels;

    [TestFixture]
    public sealed class DateOfBirthViewModelFixtures
    {
        [Test]
        public void can_not_save_when_dob_is_not_specified()
        {
            // ARRANGE
            var gestureService = new Mock<IGestureService>(MockBehavior.Strict);
            gestureService.Setup(x => x.SetBusy()).Verifiable();

            // ACT
            var viewModel = new DateOfBirthViewModel(gestureService.Object);
            
            // ASSERT
            Assert.That(viewModel.SaveCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void can_save_when_dob_specified()
        {
            // ARRANGE
            var gestureService = new Mock<IGestureService>(MockBehavior.Strict);
            gestureService.Setup(x => x.SetBusy()).Verifiable();

            var viewModel = new DateOfBirthViewModel(gestureService.Object);

            // ACT
            viewModel.Day = viewModel.Days.First().Value;
            viewModel.Month = viewModel.Months.First().Value;
            viewModel.Year = viewModel.Years.First().Value;
            
            // ASSERT
            Assert.That(viewModel.SaveCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void busy_gestures_when_saving()
        {
            // ARRANGE
            var gestureService = new Mock<IGestureService>(MockBehavior.Strict);
            gestureService.Setup(x => x.SetBusy()).Verifiable();

            var viewModel = new DateOfBirthViewModel(gestureService.Object);
            viewModel.Day = viewModel.Days.First().Value;
            viewModel.Month = viewModel.Months.First().Value;
            viewModel.Year = viewModel.Years.First().Value;

            // ACT
            viewModel.SaveCommand.Execute(null);

            // ASSERT
            gestureService.VerifyAll();
        }

        [Test]
        public void close_requested_when_saved()
        {
            // ARRANGE
            var gestureService = new Mock<IGestureService>(MockBehavior.Strict);
            gestureService.Setup(x => x.SetBusy());

            var viewModel = new DateOfBirthViewModel(gestureService.Object);
            viewModel.Day = viewModel.Days.First().Value;
            viewModel.Month = viewModel.Months.First().Value;
            viewModel.Year = viewModel.Years.First().Value;

            var closeRequested = false;
            viewModel.CloseRequested.Subscribe(x => closeRequested = true);

            // ACT
            viewModel.SaveCommand.Execute(null);

            // ASSERT
            Assert.That(closeRequested, Is.True);
        }

        [Test]
        public void disposing_clears_commands()
        {
            // ARRANGE
            var gestureService = new Mock<IGestureService>(MockBehavior.Strict);
            var viewModel = new DateOfBirthViewModel(gestureService.Object);
            
            // ACT
            viewModel.Dispose();

            // ASSERT
            var commandProperties = TestHelper.PropertiesImplementingInterface<ICommand>(viewModel);
            commandProperties.ForEach(x => Assert.That(x.GetValue(viewModel, null), Is.Null));
        }
    }
}