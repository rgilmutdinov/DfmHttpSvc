using System;
using System.IO;

namespace DfmServer.Managed.Tools
{
    public static class PathUtility
    {
        public static string GetRandomDirectory(string parentDirectory = null)
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

        public static string GetRandomFile(string directory, string extension)
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

        public static string GetValidFilename(string originalName)
        {
            char[] invalids = Path.GetInvalidFileNameChars();
            return string.Join("_", originalName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }

        public static string GetNextFileName(string filepath)
        {
            if (!File.Exists(filepath))
            {
                return filepath;
            }

            string extension = Path.GetExtension(filepath) ?? string.Empty;

            int i = 0;
            filepath = ReplaceLast(filepath, extension, "(" + ++i + ")" + extension);
            while (File.Exists(filepath))
            {
                filepath = ReplaceLast(filepath, "(" + i + ")" + extension, "(" + ++i + ")" + extension);
            }

            return filepath;
        }

        public static string GetRandomFile(string extension)
        {
            return GetRandomFile(null, extension);
        }

        private static string ReplaceLast(string source, string find, string replace)
        {
            int place = source.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
            {
                return source;
            }

            return source.Remove(place, find.Length).Insert(place, replace);
        }
    }
}
