namespace Simple.Wpf.Template.Tests
{
    using System;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Services;
    using ObservableExtensions = Extensions.ObservableExtensions;

    public abstract class BaseViewModelFixtures
    {
        public MockSchedulerService SchedulerService { get; private set; }

        public TestScheduler TestScheduler { get; private set; }

        public Mock<IGestureService> GestureService { get; private set; }

        [OneTimeSetUp]
        public void BaseSetup()
        {
            GestureService = new Mock<IGestureService>();
            GestureService.Setup(x => x.SetBusy()).Verifiable();

            ObservableExtensions.GestureService = GestureService.Object;

            TestScheduler = new TestScheduler();
            TestScheduler.AdvanceTo(DateTimeOffset.Now.Ticks);

            SchedulerService = new MockSchedulerService(TestScheduler);
        }
    }
}