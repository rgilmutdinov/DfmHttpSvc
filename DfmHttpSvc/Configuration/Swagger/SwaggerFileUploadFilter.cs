using System.Collections.Generic;
using System.Linq;
using DfmCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DfmHttpSvc.Configuration.Swagger
{
    public class SwaggerFileUploadFilter : IOperationFilter
    {
        private const string FormDataMimeType = "multipart/form-data";

        public void Apply(Operation operation, OperationFilterContext context)
        {
            string method = context.ApiDescription.HttpMethod.ToLower();
            if (!method.Equals("post") && !method.Equals("put"))
            {
                return;
            }

            List<ParameterDescriptor> fileParams = context.ApiDescription.ActionDescriptor.Parameters
                .Where(n => n.ParameterType == typeof(IFormFile))
                .ToList();

            if (fileParams.IsNullOrEmpty())
            {
                return;
            }

            List<IParameter> operationParams = operation.Parameters.ToList();
            foreach (ParameterDescriptor fileParam in fileParams)
            {
                int paramIdx = operationParams.FindIndex(n => n.Name == fileParam.Name);
                if (paramIdx >= 0)
                {
                    IParameter parameter = operation.Parameters[paramIdx];
                    operation.Parameters[paramIdx] = new NonBodyParameter
                    {
                        Name        = parameter.Name,
                        In          = "formData",
                        Description = parameter.Description,
                        Required    = parameter.Required,
                        Type        = "file"
                    };
                }
            }

            operation.Consumes.Add(FormDataMimeType);
        }
    }
}
