using System;
using System.Collections.Generic;
using DfmCore.Extensions;

namespace DfmCore
{
    public class Area
    {
        public Area()
        {

        }

        public Area(string name, string description)
        {
            Name        = name;
            Description = description;
        }

        public Area(List<string> pathParts)
        {
            Name = string.Join("\r", pathParts);
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public string ShortName
        {
            get
            {
                if (Name.IsNullOrEmpty())
                {
                    return Name;
                }

                string[] parts = PathParts;
                if (parts.Length > 0)
                {
                    return parts[parts.Length - 1];
                }

                return Name;
            }
        }

        public string[] PathParts
        {
            get
            {
                if (Name.IsNullOrEmpty())
                {
                    return new string[0];
                }

                return Name.Split(new[] {"\r"}, StringSplitOptions.None);
            }
        }
    }
}
