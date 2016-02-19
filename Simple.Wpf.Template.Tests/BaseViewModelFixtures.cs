namespace Simple.Wpf.Template.Tests
{
    using Microsoft.Reactive.Testing;
    using Moq;
    using Services;

    public abstract class BaseViewModelFixtures
    {
        protected BaseViewModelFixtures()
        {
            GestureService = new Mock<IGestureService>();
            GestureService.Setup(x => x.SetBusy()).Verifiable();

            Extensions.ObservableExtensions.GestureService = GestureService.Object;

            TestScheduler = new TestScheduler();
            SchedulerService = new MockSchedulerService(TestScheduler);
        }

        public MockSchedulerService SchedulerService { get; }

        public TestScheduler TestScheduler { get; }

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