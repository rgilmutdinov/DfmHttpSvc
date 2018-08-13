using System;
using System.Globalization;
using DFMServer;
using FieldInfo = DfmServer.Managed.FieldInfo;

namespace Workflow.Expressions
{
    public abstract class DbTranslator
    {
        public abstract string GetDbDate(int year, int month, int day, int hour, int minute, int second);
        public abstract string GetToDbTime(int hour, int minute, int second);

        public abstract string FieldToInt(FieldInfo fieldInfo);
        public abstract string FieldLength(FieldInfo fieldInfo);
        public abstract string FieldToDate(FieldInfo fieldInfo);
    }

    public class MssqlTranslator : DbTranslator
    {
        public override string GetDbDate(int year, int month, int day, int hour, int minute, int second)
        {
            return $"CONVERT(datetime, '{year:D4}-{month:D2}-{day:D2}T{hour:D2}:{minute:D2}:{second:D2}', 126)";
        }

        public override string GetToDbTime(int hour, int minute, int second)
        {
            return $"CONVERT(datetime, '{hour:D2}:{minute:D2}:{second:D2}')";
        }

        public override string FieldToInt(FieldInfo fieldInfo)
        {
            return $"CAST(X{fieldInfo.Name} AS INT)";
        }

        public override string FieldLength(FieldInfo fieldInfo)
        {
            return $"LEN(X{fieldInfo.Name})";
        }

        public override string FieldToDate(FieldInfo fieldInfo)
        {
            return $"CONVERT(datetime, X{fieldInfo.Name}, 126)";
        }
    }

    public class FirebirdTranslator : DbTranslator
    {
        public override string GetDbDate(int year, int month, int day, int hour, int minute, int second)
        {
            return $"CAST('{year:D4}-{month:D2}-{day:D2} {hour:D2}:{minute:D2}:{second:D2}' AS DATE)";
        }

        public override string GetToDbTime(int hour, int minute, int second)
        {
            DateTime now = DateTime.Now;
            DateTime dt = new DateTime(now.Year, now.Month, now.Day, hour, minute, second);

            return dt.TimeOfDay.TotalDays.ToString(CultureInfo.InvariantCulture);
        }

        public override string FieldToInt(FieldInfo fieldInfo)
        {
            return $"CAST(X{fieldInfo.Name} AS INTEGER)";
        }

        public override string FieldLength(FieldInfo fieldInfo)
        {
            if (fieldInfo.Type == DFM_FIELD_TYPE.DFM_FT_INTEGER)
            {
                return $"CHAR_LENGTH(CAST(X{fieldInfo.Name} AS INTEGER))";
            }

            if (fieldInfo.Type == DFM_FIELD_TYPE.DFM_FT_DECIMAL)
            {
                return $"(CHAR_LENGTH(CAST(X{fieldInfo.Name} AS INTEGER)) + {fieldInfo.Precision + 1})";
            }

            return $"CHAR_LENGTH(X{fieldInfo.Name})";
        }

        public override string FieldToDate(FieldInfo fieldInfo)
        {
            return $"CAST(REPLACE(X{fieldInfo.Name}, 'T', ' ') AS DATE)";
        }
    }

    public class OracleTranslator : DbTranslator
    {
        public override string GetDbDate(int year, int month, int day, int hour, int minute, int second)
        {
            return $"TO_DATE('{year:D4}-{month:D2}-{day:D2}T{hour:D2}:{minute:D2}:{second:D2}', 'YYYY-MM-DD\"T\"HH24:MI:SS')";
        }

        public override string GetToDbTime(int hour, int minute, int second)
        {
            DateTime now = DateTime.Now;
            DateTime dt = new DateTime(now.Year, now.Month, now.Day, hour, minute, second);

            return dt.TimeOfDay.TotalDays.ToString(CultureInfo.InvariantCulture);
        }

        public override string FieldToInt(FieldInfo fieldInfo)
        {
            return $"TO_NUMBER(X{fieldInfo.Name})";
        }

        public override string FieldLength(FieldInfo fieldInfo)
        {
            return $"LENGTH(X{fieldInfo.Name})";
        }

        public override string FieldToDate(FieldInfo fieldInfo)
        {
            return $"TO_DATE(X{fieldInfo.Name}, 'YYYY-MM-DD\"T\"HH24:MI:SS')";
        }
    }
}
