namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using Extensions;
    using Helpers;
    using Models;
    using Newtonsoft.Json;
    using Rest;
    using Services;

    public sealed class ModifyResourceViewModel : CloseableViewModel, IModifyResourceViewModel
    {
        private string _json;

        public ModifyResourceViewModel(Metadata metadata, IRestClient restClient, ISchedulerService schedulerService)
        {
            Path = metadata.Url.ToString().Replace(Constants.Server.BaseUri, string.Empty);

            Observable.Return(Unit.Default)
                .ActivateGestures()
                .SelectMany(x => restClient.GetAsync<object>(metadata.Url).ToObservable(), (x, y) => y)
                .Select(x => JsonConvert.SerializeObject(x.Resource, Formatting.Indented))
                .ObserveOn(schedulerService.Dispatcher)
                .Do(x => Json = x)
                .SelectMany(x => Confirmed, (x, y) => y)
                .ActivateGestures()
                .ObserveOn(schedulerService.TaskPool)
                .SelectMany(x => restClient.PutAsync(metadata.Url, new Resource(Json)).ToObservable(), (x, y) => y)
                .Take(1)
                .Subscribe()
                .DisposeWith(this);
        }

        public string Path { get; }

        public string Json
        {
            get { return _json; }
            set { SetPropertyAndNotify(ref _json, value, () => Json); }
        }

        protected override IObservable<bool> InitialiseCanConfirm()
        {
            return this.ObservePropertyChanged(x => Json)
                .Select(x => JsonHelper.IsValid(Json))
                .StartWith(false);
        }
    }
}