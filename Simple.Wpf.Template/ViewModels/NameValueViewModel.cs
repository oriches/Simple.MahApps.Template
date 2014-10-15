namespace Simple.Wpf.Template.ViewModels
{
    public sealed class NameValueViewModel<T> where T : struct 
    {
        public NameValueViewModel(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public T Value { get; private set; }
    }
}