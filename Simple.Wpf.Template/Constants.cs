using System;

namespace Simple.Wpf.Template
{
    public static class Constants
    {
        public static class UI
        {
            public const string ExceptionTitle = "whoops - something's gone wrong!";

            public static class Diagnostics
            {
                public const string DefaultCpuString = "CPU: 00 %";
                public const string DefaultManagedMemoryString = "Managed Memory: 00 Mb";
                public const string DefaultTotalMemoryString = "Total Memory: 00 Mb";
                public static readonly TimeSpan Heartbeat = TimeSpan.FromSeconds(5);
                public static readonly TimeSpan UiFreeze = TimeSpan.FromMilliseconds(500);
                public static readonly TimeSpan UiFreezeTimer = TimeSpan.FromMilliseconds(333);

                public static readonly TimeSpan DiagnosticsLogInterval = TimeSpan.FromSeconds(1);
                public static readonly TimeSpan DiagnosticsIdleBuffer = TimeSpan.FromMilliseconds(666);
                public static readonly TimeSpan DiagnosticsCpuBuffer = TimeSpan.FromMilliseconds(666);
                public static readonly TimeSpan DiagnosticsSubscriptionDelay = TimeSpan.FromMilliseconds(1000);
            }
        }

        public static class Server
        {
            public static readonly string BaseUri = $"http://{Environment.MachineName}:1337/";

            public static readonly Uri MetadataUrl = new Uri(BaseUri + "metadata");

            public static class Hearbeat
            {
                public static readonly Uri Url = new Uri(BaseUri + "heartbeat");

                public static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
                public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(7);
            }
        }
    }
}