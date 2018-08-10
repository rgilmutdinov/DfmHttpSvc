using System;
using System.Linq;
using DFMServer;
using FieldInfo = DfmServer.Managed.FieldInfo;
using Volume = DfmServer.Managed.Volume;

namespace Workflow.Expressions.Resolvers
{
    public interface IDbResolver
    {
        string ResolveField(string fieldName);
        string ResolveFldlen(string fieldName);
        string ResolveArg(Argument arg);
    }

    public class BasicDbResolver : IDbResolver
    {
        public string ResolveField(string fieldName)
        {
            return "X" + fieldName;
        }

        public string ResolveFldlen(string fieldName)
        {
            return $"LEN({fieldName})";
        }

        public string ResolveArg(Argument arg)
        {
            return arg.ToString();
        }
    }

    public class DbResolver : IDbResolver
    {
        private readonly Volume _volume;
        private readonly DbTranslator _translator;

        public DbResolver(Volume volume, DbTranslator translator)
        {
            this._volume = volume;
            this._translator = translator;
        }

        public string ResolveField(string fieldName)
        {
            FieldInfo fieldInfo = this._volume.Fields.First(f => 
                string.Equals(f.Name, fieldName, StringComparison.OrdinalIgnoreCase)
            );

            if (fieldInfo.Type != DFM_FIELD_TYPE.DFM_FT_MEMO && fieldInfo.Type != DFM_FIELD_TYPE.DFM_FT_STRING)
            {
                return "X" + fieldName;
            }

            return this._translator.FieldToInt(fieldInfo);
        }

        public string ResolveFldlen(string fieldName)
        {
            FieldInfo fieldInfo = this._volume.Fields.First(f =>
                string.Equals(f.Name, fieldName, StringComparison.OrdinalIgnoreCase)
            );

            return this._translator.FieldLength(fieldInfo);
        }

        public string ResolveArg(Argument arg)
        {
            if (arg.IsDate)
            {
                DateTime date = arg.ToDate().Value;
                return this._translator.GetDbDate(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            }

            if (arg.IsTime)
            {
                int seconds = arg.ToTime();
                DateTime dt = DateTime.Today.AddSeconds(seconds);
                return this._translator.GetToDbTime(dt.Hour, dt.Minute, dt.Second);
            }

            return arg.ToString();
        }
    }
}
