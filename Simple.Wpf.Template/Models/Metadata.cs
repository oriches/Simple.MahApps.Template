using System;

namespace Simple.Wpf.Template.Models
{
    public sealed class Metadata : IEquatable<Metadata>
    {
        public Metadata(Uri url, bool immutable)
        {
            Url = url;
            Immutable = immutable;
        }

        public Uri Url { get; }

        public bool Immutable { get; }

        public bool Equals(Metadata other) => Url == other.Url;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Metadata && Equals((Metadata)obj);
        }

        public override int GetHashCode() => Url?.GetHashCode() ?? 0;

        public static bool operator ==(Metadata left, Metadata right) => left != null && left.Equals(right);

        public static bool operator !=(Metadata left, Metadata right) => left != null && !left.Equals(right);
    }
}