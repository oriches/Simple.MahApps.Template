namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Threading.Tasks;
    using Extensions;
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Rest;

    public sealed class AddResourceViewModel : CloseableViewModel, IAddResourceViewModel
    {
        private readonly IEnumerable<string> _urls;
        private string _json;

        private string _path;

        public AddResourceViewModel(IEnumerable<Metadata> metadata, IRestClient restClient)
        {
            _urls = metadata.Select(x => x.Url.ToString());

            Confirmed
                .SelectMany(x => restClient.PostAsync(BuildUrl(), new Resource(_json)).ToObservable(), (x, y) => y)
                .Take(1)
                .Subscribe()
                .DisposeWith(this);
        }

        public string Path
        {
            get { return _path; }
            set { SetPropertyAndNotify(ref _path, value, () => Path); }
        }

        public string Json
        {
            get { return _json; }
            set { SetPropertyAndNotify(ref _json, value, () => Json); }
        }

        protected override IObservable<bool> InitialiseCanConfirm()
        {
            return this.ObservePropertyChanged(x => Path, x => Json)
                .Select(x => !string.IsNullOrEmpty(Path) && IsPathAvailable(Path) && IsValidJson(Json))
                .StartWith(false);
        }

        private static bool IsValidJson(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            try
            {
                JToken.Parse(value);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        private bool IsPathAvailable(string value)
        {
            var url = Constants.Server.BaseUri + value;

            var found = _urls.Any(x => string.Equals(x, url, StringComparison.InvariantCultureIgnoreCase));
            return !found;
        }

        private Uri BuildUrl()
        {
            var trimmedPath = Path.Replace('\\', '/')
                .TrimStart('/');

            return new Uri(Constants.Server.BaseUri + trimmedPath);
        }
    }
}