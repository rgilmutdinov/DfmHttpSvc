using System;
using System.IO;

namespace DfmCore.Tools
{
    public static class RandomPath
    {
        public static string GetDirectory(string parentDirectory = null)
        {
            if (string.IsNullOrEmpty(parentDirectory))
            {
                parentDirectory = Path.GetTempPath();
            }

            string location;
            do
            {
                location = Path.Combine(parentDirectory, Guid.NewGuid().ToString());
            } while (Directory.Exists(location));

            return location;
        }

        public static string GetFile(string directory, string extension)
        {
            if (string.IsNullOrEmpty(directory))
            {
                directory = Path.GetTempPath();
            }

            if (string.IsNullOrEmpty(extension))
            {
                extension = ".tmp";
            }

            else if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string location;
            do
            {
                location = Path.Combine(directory, Guid.NewGuid() + extension);
            } while (File.Exists(location));

            return location;
        }

        public static string GetFile(string extension)
        {
            return GetFile(null, extension);
        }
    }
}
