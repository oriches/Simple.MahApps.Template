namespace Simple.Wpf.Template.Models
{
    using System;

    public sealed class Resource : IEquatable<Resource>
    {
        public Resource(Uri url, bool immutable)
        {
            Url = url;
            Immutable = immutable;
        }

        public bool Equals(Resource other)
        {
            return Url == other.Url;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Resource && Equals((Resource) obj);
        }

        public override int GetHashCode()
        {
            return Url?.GetHashCode() ?? 0;
        }

        public static bool operator ==(Resource left, Resource right)
        {
            return left != null && left.Equals(right);
        }

        public static bool operator !=(Resource left, Resource right)
        {
            return left != null && !left.Equals(right);
        }

        public Uri Url { get; }

        public bool Immutable { get; }
    }
}