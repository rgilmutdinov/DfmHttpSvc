using System;
using System.Collections.Generic;
using System.Linq;
using DfmCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DfmHttpSvc.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ArrayInputAttribute : ActionFilterAttribute
    {
        public ArrayInputAttribute(string parameterName, Type parameterType, char separator)
        {
            ParameterName = parameterName;
            ParameterType = parameterType;
            Separator     = separator;
        }

        public Type   ParameterType { get; }
        public string ParameterName { get; }
        public char   Separator     { get; }

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            HttpRequest request = actionContext.HttpContext.Request;
            if (request == null)
            {
                return;
            }

            string queryString = actionContext.HttpContext.Request?.QueryString.Value;
            if (queryString == null || queryString.IsNullOrEmpty())
            {
                return;
            }

            Dictionary<string, string> queryParams = GetRawParameters(queryString);

            if (queryParams.TryGetValue(ParameterName, out string paramValue))
            {
                string[] values = paramValue.Split(Separator);

                List<string> decodedValues = new List<string>();
                foreach (string value in values)
                {
                    decodedValues.Add(Uri.UnescapeDataString(value));
                }

                actionContext.ActionArguments[ParameterName] = decodedValues;
            }
        }

        private Dictionary<string, string> GetRawParameters(string queryString)
        {
            if (queryString == null)
            {
                throw new ArgumentNullException(nameof(queryString));
            }

            if (queryString.Length == 0)
            {
                return new Dictionary<string, string>();
            }

            return queryString.TrimStart('?')
                .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(parts => parts[0], parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length > 1 ? parts[1] : ""))
                .ToDictionary(grouping => grouping.Key, grouping => string.Join(Separator.ToString(), grouping));
        }
    }
}
