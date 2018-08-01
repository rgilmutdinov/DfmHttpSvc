using System;
using System.Collections.Generic;
using System.Globalization;

namespace Workflow.Expressions
{
    public class DateUtils
    {
        private static readonly List<string> DateFormats = new List<string>();

        static DateUtils()
        {
            AddDateFormat("yyyy/MM/dd'T'HH:mm:ss");
            AddDateFormat("yyyy-MM-dd'T'HH:mm:ss");
            AddDateFormat("dd-MM-yyyy'T'HH:mm:ss");
            AddDateFormat("dd/MM/yyyy'T'HH:mm:ss");

            AddDateFormat("yyyy/MM/dd");
            AddDateFormat("yyyy-MM-dd");
            AddDateFormat("dd-MM-yyyy");
            AddDateFormat("dd/MM/yyyy");
        }

        private static void AddDateFormat(string dateFormat)
        {
            DateFormats.Add(dateFormat);
        }

        private static bool TryParse(string input, string format, out DateTime dateTime)
        {
            return DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        public static DateTime? MultiParseDate(string input)
        {
            return MultiParseDate(input, DateFormats);
        }

        public static bool TryMultiParseDate(string input, out DateTime dateTime)
        {
            return TryMultiParseDate(input, DateFormats, out dateTime);
        }

        public static bool TryMultiParseDate(string input, List<string> formats, out DateTime dateTime)
        {
            foreach (string format in formats)
            {
                if (TryParse(input, format, out dateTime))
                {
                    return true;
                }
            }

            dateTime = DateTime.MinValue;
            return false;
        }

        public static DateTime? MultiParseDate(string input, List<string> formats)
        {
            foreach (string format in formats)
            {
                if (TryParse(input, format, out DateTime dateTime))
                {
                    return dateTime;
                }
            }
            return null;
        }

        public static long MultiParseTime(string input)
        {
            DateTime dateTime = DateTime.ParseExact(input, "HH:mm:ss", CultureInfo.InvariantCulture);
            return (long) dateTime.TimeOfDay.TotalSeconds;
        }

        public static bool TryMultiParseTime(string input, out long totalSeconds)
        {
            totalSeconds = -1;

            bool ok = DateTime.TryParseExact(input, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
            if (ok)
            {
                totalSeconds = (long) dateTime.TimeOfDay.TotalSeconds;
            }

            return ok;
        }
    }
}
