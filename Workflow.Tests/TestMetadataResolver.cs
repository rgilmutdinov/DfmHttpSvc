using System;
using System.Globalization;
using DFMServer;
using Workflow.Expressions;
using Workflow.Expressions.Resolvers;
using FieldInfo = DfmServer.Managed.FieldInfo;

namespace Workflow.Tests
{
    public class TestMetadataResolver : IMetadataResolver
    {
        private const string DateFormat = "yyyy-MM-dd";
        private const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss";

        public Argument ResolveVariable(string variableName)
        {
            if ("SYSTIME".Equals(variableName, StringComparison.OrdinalIgnoreCase))
            {
                return new Argument(DateTime.ParseExact("2013-10-10", DateFormat, CultureInfo.InvariantCulture));
            }
        
            if ("SYSTIME_YMD".Equals(variableName, StringComparison.OrdinalIgnoreCase))
            {
                return new Argument(DateTime.ParseExact("2013-10-10T10:10:10", DateTimeFormat, CultureInfo.InvariantCulture));
            }
        
            return Argument.Null;
        }

        public Argument ResolveFieldValue(string fieldName)
        {
            switch (fieldName)
            {
                case "INTFLD1": return new Argument(1);
                case "INTFLD2": return new Argument(2);
                case "DBLFLD1": return new Argument(1.5);
                case "DBLFLD2": return new Argument(2.5);
                case "STRFLD1": return new Argument("str1");
                case "STRFLD2": return new Argument("str2");
                case "DATFLD1": return new Argument(DateTime.ParseExact("2013-10-05", DateFormat, CultureInfo.InvariantCulture));
                case "DATFLD2": return new Argument(DateTime.ParseExact("2013-05-10", DateFormat, CultureInfo.InvariantCulture));
                case "DATSTRFLD": return new Argument(new DateTime(2016, 02, 02));
                default: return Argument.Null;
            }
        }

        public FieldInfo GetField(string fieldName)
        {
            switch (fieldName)
            {
                case "INTFLD1": return CreateFieldInfo(fieldName, DFM_FIELD_TYPE.DFM_FT_INTEGER);
                case "INTFLD2": return CreateFieldInfo(fieldName, DFM_FIELD_TYPE.DFM_FT_INTEGER);
                case "DBLFLD1": return CreateFieldInfo(fieldName, DFM_FIELD_TYPE.DFM_FT_DECIMAL);
                case "DBLFLD2": return CreateFieldInfo(fieldName, DFM_FIELD_TYPE.DFM_FT_DECIMAL);
                case "STRFLD1": return CreateFieldInfo(fieldName, DFM_FIELD_TYPE.DFM_FT_STRING);
                case "STRFLD2": return CreateFieldInfo(fieldName, DFM_FIELD_TYPE.DFM_FT_STRING);
                case "DATFLD1": return CreateFieldInfo(fieldName, DFM_FIELD_TYPE.DFM_FT_DATE);
                case "DATFLD2": return CreateFieldInfo(fieldName, DFM_FIELD_TYPE.DFM_FT_DATE);
                case "DATSTRFLD": return CreateFieldInfo(fieldName, DFM_FIELD_TYPE.DFM_FT_STRING);
                default:
                    throw new Exception("Unknown field: " + fieldName);
            }
        }

        private FieldInfo CreateFieldInfo(string name, DFM_FIELD_TYPE type)
        {
            FieldInfo fieldInfo = new FieldInfo
            {
                Name = name,
                Type = type
            };

            return fieldInfo;
        }
    }
}
