namespace Simple.Wpf.Template.Models
{
    using System;
    using Newtonsoft.Json;

    public sealed class Resource : IEquatable<Resource>
    {
        public Resource(string json)
        {
            Json = json;
        }

        [JsonProperty(PropertyName = "json")]
        public string Json { get; }

        public bool Equals(Resource other)
        {
            return Json == other.Json;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Resource && Equals((Resource) obj);
        }

        public override int GetHashCode()
        {
            return Json?.GetHashCode() ?? 0;
        }

        public static bool operator ==(Resource left, Resource right)
        {
            return left != null && left.Equals(right);
        }

        public static bool operator !=(Resource left, Resource right)
        {
            return left != null && !left.Equals(right);
        }
    }
}