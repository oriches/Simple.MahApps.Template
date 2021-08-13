using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Simple.Wpf.Template.Helpers
{
    public static class JsonHelper
    {
        public static bool IsValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

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