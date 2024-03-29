using System.ComponentModel;

namespace Simple.Wpf.Template.Models
{
    public enum MemoryUnits
    {
        [Description("bytes")] Bytes = 1,
        [Description("Kb")] Kilo = 1024,
        [Description("Mb")] Mega = 1024 * 1000,
        [Description("Gb")] Giga = 1024 * 1000 * 1000
    }
}