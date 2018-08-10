using System;
using System.Globalization;
using Workflow.Expressions;
using Workflow.Expressions.Resolvers;

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
    }
}
