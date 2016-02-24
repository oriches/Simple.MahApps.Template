namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive.Linq;
    using Extensions;
    using Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public sealed class ModifyResourceViewModel : CloseableViewModel, IModifyResourceViewModel
    {
        private readonly Metadata _metadata;
        private string _json;

        public ModifyResourceViewModel(Metadata metadata)
        {
            _metadata = metadata;
            Path = metadata.Url.ToString().Replace(Constants.Server.BaseUri, string.Empty);
        }

        public string Path { get; }

        public string Json
        {
            get { return _json; }
            set { SetPropertyAndNotify(ref _json, value, () => Json); }
        }

        protected override IObservable<bool> InitialiseCanConfirm()
        {
            return this.ObservePropertyChanged(x => Path, x => Json)
                .Select(x => IsValidJson(Json))
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
    }
}