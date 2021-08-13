using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Newtonsoft.Json;
using Simple.Rest.Common;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Helpers;
using Simple.Wpf.Template.Models;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.ViewModels
{
    public sealed class ModifyResourceViewModel : CloseableViewModel, IModifyResourceViewModel
    {
        private readonly IRestClient _restClient;
        private string _json;

        public ModifyResourceViewModel(Metadata metadata, IRestClient restClient, ISchedulerService schedulerService)
        {
            _restClient = restClient;

            Path = metadata.Url.ToString()
                .Replace(Constants.Server.BaseUri, string.Empty);

            Observable.Return(Unit.Default)
                .ActivateGestures()
                .ObserveOn(schedulerService.TaskPool)
                .SelectMany(x => ObserveGetResource(metadata), (x, y) => y)
                .ObserveOn(schedulerService.Dispatcher)
                .Do(x => Json = x)
                .AsUnit()
                .Merge(Confirmed)
                .ActivateGestures()
                .ObserveOn(schedulerService.TaskPool)
                .SelectMany(x => ObservePutResource(metadata), (x, y) => y)
                .Take(1)
                .Subscribe()
                .DisposeWith(this);
        }

        public string Path { get; }

        public string Json
        {
            get => _json;
            set { SetPropertyAndNotify(ref _json, value, () => Json); }
        }

        protected override IObservable<bool> InitialiseCanConfirm()
        {
            return this.ObservePropertyChanged(x => Json)
                .Select(x => JsonHelper.IsValid(Json))
                .StartWith(false);
        }

        private IObservable<string> ObserveGetResource(Metadata metadata)
        {
            return _restClient.GetAsync<object>(metadata.Url)
                .ToObservable()
                .Take(1)
                .Select(x => JsonConvert.SerializeObject(x.Resource, Formatting.Indented));
        }

        private IObservable<Unit> ObservePutResource(Metadata metadata)
        {
            return _restClient.PutAsync(metadata.Url, new Resource(Json))
                .ToObservable()
                .Take(1)
                .AsUnit();
        }
    }
}