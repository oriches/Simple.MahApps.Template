namespace Simple.Wpf.Template.Tests
{
    using Moq;
    using Services;

    public abstract class BaseViewModelFixtures
    {
        protected BaseViewModelFixtures()
        {
            GestureService = new Mock<IGestureService>();
            GestureService.Setup(x => x.SetBusy()).Verifiable();

            Extensions.ObservableExtensions.GestureService = GestureService.Object;
        }

        public Mock<IGestureService> GestureService { get; }
    }

    public abstract class BaseServiceFixtures
    {
        protected BaseServiceFixtures()
        {
            GestureService = new Mock<IGestureService>();
            GestureService.Setup(x => x.SetBusy()).Verifiable();

            Extensions.ObservableExtensions.GestureService = GestureService.Object;
        }

        public Mock<IGestureService> GestureService { get; }
    }
}