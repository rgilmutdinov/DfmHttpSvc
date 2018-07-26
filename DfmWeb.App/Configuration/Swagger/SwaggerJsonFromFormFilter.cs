using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DfmServer.Managed.Extensions;
using DfmWeb.App.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DfmWeb.App.Configuration.Swagger
{
    public class SwaggerJsonFromFormFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            List<ControllerParameterDescriptor> jsonParams = context.ApiDescription.ActionDescriptor.Parameters
                .OfType<ControllerParameterDescriptor>()
                .Where(p => p.ParameterInfo.GetCustomAttribute(typeof(JsonFromFormAttribute)) != null)
                .ToList();

            if (jsonParams.IsNullOrEmpty())
            {
                return;
            }

            List<IParameter> operationParams = operation.Parameters.ToList();
            foreach (ControllerParameterDescriptor jsonParam in jsonParams)
            {
                int paramIdx = operationParams.FindIndex(n => n.Name == jsonParam.Name);
                if (paramIdx >= 0)
                {
                    IParameter parameter = operation.Parameters[paramIdx];
                    operation.Parameters[paramIdx] = new NonBodyParameter
                    {
                        Name        = parameter.Name,
                        In          = "formData",
                        Description = parameter.Description,
                        Required    = parameter.Required
                    };
                }
            }
        }
    }
}