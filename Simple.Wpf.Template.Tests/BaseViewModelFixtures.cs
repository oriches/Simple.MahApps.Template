using System;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Simple.Wpf.Template.Services;
using ObservableExtensions = Simple.Wpf.Template.Extensions.ObservableExtensions;

namespace Simple.Wpf.Template.Tests
{
    public abstract class BaseViewModelFixtures
    {
        public MockSchedulerService SchedulerService { get; private set; }

        public TestScheduler TestScheduler { get; private set; }

        public Mock<IGestureService> GestureService { get; private set; }

        [SetUp]
        public void BaseSetup()
        {
            GestureService = new Mock<IGestureService>();
            GestureService.Setup(x => x.SetBusy())
                .Verifiable();

            ObservableExtensions.GestureService = GestureService.Object;

            TestScheduler = new TestScheduler();
            TestScheduler.AdvanceTo(DateTimeOffset.Now.Ticks);

            SchedulerService = new MockSchedulerService(TestScheduler);
        }
    }
}