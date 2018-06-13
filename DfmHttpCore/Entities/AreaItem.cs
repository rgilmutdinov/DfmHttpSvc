using System;
using System.Linq;
using DfmCore;

namespace DfmHttpCore.Entities
{
    public class AreaItem
    {
        public const char PathSeparator = ',';

        public AreaItem(Area area)
        {
            // escape area name parts and combine with separator
            Path        = string.Join(PathSeparator.ToString(), area.PathParts.Select(Uri.EscapeDataString));
            Description = area.Description;
            Name        = area.ShortName;
        }

        public string Path        { get; }
        public string Description { get; }
        public string Name        { get; }
    }
}
