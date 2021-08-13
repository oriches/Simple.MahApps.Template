using Moq;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Tests
{
    public abstract class BaseServiceFixtures
    {
        protected BaseServiceFixtures()
        {
            GestureService = new Mock<IGestureService>();
            GestureService.Setup(x => x.SetBusy())
                .Verifiable();

            ObservableExtensions.GestureService = GestureService.Object;
        }

        public Mock<IGestureService> GestureService { get; }
    }
}