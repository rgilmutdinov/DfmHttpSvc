using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DfmHttpSvc.Configuration.Swagger
{
    public class SwaggerFileUploadFilter : IOperationFilter
    {
        private const string FormDataMimeType = "multipart/form-data";

        private static readonly string[] FormFilePropertyNames =
            typeof(IFormFile).GetTypeInfo().DeclaredProperties.Select(p => p.Name).ToArray();

        public void Apply(Operation operation, OperationFilterContext context)
        {
            IList<IParameter> parameters = operation.Parameters;
            if (parameters == null || parameters.Count == 0)
            {
                return;
            }

            List<string> formFileParamNames = new List<string>();
            List<string> formFileSubParamNames = new List<string>();

            foreach (ParameterDescriptor actionParameter in context.ApiDescription.ActionDescriptor.Parameters)
            {
                string[] properties =
                    actionParameter.ParameterType.GetProperties()
                        .Where(p => p.PropertyType == typeof(IFormFile))
                        .Select(p => p.Name)
                        .ToArray();

                if (properties.Length != 0)
                {
                    formFileParamNames.AddRange(properties);
                    formFileSubParamNames.AddRange(properties);

                    continue;
                }

                if (actionParameter.ParameterType != typeof(IFormFile))
                {
                    continue;
                }

                formFileParamNames.Add(actionParameter.Name);
            }

            if (!formFileParamNames.Any())
            {
                return;
            }

            IList<string> consumes = operation.Consumes;
            consumes.Clear();
            consumes.Add(FormDataMimeType);

            foreach (IParameter parameter in parameters.ToArray())
            {
                if (!(parameter is NonBodyParameter) || parameter.In != "formData")
                {
                    continue;
                }

                if (formFileSubParamNames.Any(p => parameter.Name.StartsWith(p + ".")) || FormFilePropertyNames.Contains(parameter.Name))
                {
                    parameters.Remove(parameter);
                }
            }

            foreach (string paramName in formFileParamNames)
            {
                parameters.Add(new NonBodyParameter
                {
                    Name = paramName,
                    Type = "file",
                    In = "formData"
                });
            }
        }
    }
}
