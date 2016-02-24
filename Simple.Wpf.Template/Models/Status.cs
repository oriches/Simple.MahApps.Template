namespace Simple.Wpf.Template.Models
{
    using System;

    public sealed class Status : IEquatable<Status>
    {
        public Status(Exception exception)
        {
            Exception = exception;
        }

        public Status(string timestamp)
        {
            Timestamp = timestamp;
        }

        public Exception Exception { get; }
        public string Timestamp { get; }
        public bool IsOnline => !string.IsNullOrEmpty(Timestamp);
        public bool HasTimedOut => Exception != null && Exception.GetType() == typeof (TimeoutException);

        public bool Equals(Status other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Exception, other.Exception) && string.Equals(Timestamp, other.Timestamp);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Status && Equals((Status) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Exception?.GetHashCode() ?? 0)*397) ^ (Timestamp?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(Status left, Status right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Status left, Status right)
        {
            return !Equals(left, right);
        }
    }
}