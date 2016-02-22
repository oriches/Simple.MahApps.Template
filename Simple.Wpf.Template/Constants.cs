namespace Simple.Wpf.Template
{
    using System;

    public static class Constants
    {
        public static class UI
        {
            public static class Diagnostics
            {
                public static readonly TimeSpan Heartbeat = TimeSpan.FromSeconds(5);
                public static readonly TimeSpan UiFreeze = TimeSpan.FromMilliseconds(500);
                public static readonly TimeSpan UiFreezeTimer = TimeSpan.FromMilliseconds(333);

                public static readonly TimeSpan DiagnosticsLogInterval = TimeSpan.FromSeconds(1);
                public static readonly TimeSpan DiagnosticsIdleBuffer = TimeSpan.FromMilliseconds(666);
                public static readonly TimeSpan DiagnosticsCpuBuffer = TimeSpan.FromMilliseconds(666);
                public static readonly TimeSpan DiagnosticsSubscriptionDelay = TimeSpan.FromMilliseconds(1000);

                public const string DefaultCpuString = "CPU: 00 %";
                public const string DefaultManagedMemoryString = "Managed Memory: 00 Mb";
                public const string DefaultTotalMemoryString = "Total Memory: 00 Mb";
            }
        }

        public static class Server
        {
            public static class Hearbeat
            {
                public static readonly Uri Url = new Uri("http://localhost:1337/heartbeat");

                public static readonly TimeSpan Interval = TimeSpan.FromSeconds(5);
                public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(7);
            }

            public static readonly Uri ResourcesUrl = new Uri("http://localhost:1337/resources");
        }
    }
}
