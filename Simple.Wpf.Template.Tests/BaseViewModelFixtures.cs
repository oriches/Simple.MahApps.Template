namespace Simple.Wpf.Template.Tests
{
    using System;
    using Microsoft.Reactive.Testing;
    using Moq;
    using NUnit.Framework;
    using Services;

    public abstract class BaseViewModelFixtures
    {
        [OneTimeSetUp]
        public void BaseSetup()
        {
            GestureService = new Mock<IGestureService>();
            GestureService.Setup(x => x.SetBusy()).Verifiable();

            Extensions.ObservableExtensions.GestureService = GestureService.Object;

            TestScheduler = new TestScheduler();
            TestScheduler.AdvanceTo(DateTimeOffset.Now.Ticks);

            SchedulerService = new MockSchedulerService(TestScheduler);
        }

        public MockSchedulerService SchedulerService { get; private set; }

        public TestScheduler TestScheduler { get; private set; }

        public Mock<IGestureService> GestureService { get; private set; }
    }
}