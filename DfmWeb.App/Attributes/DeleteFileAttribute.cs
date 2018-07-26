using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DfmWeb.App.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DeleteFileAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            context?.HttpContext.Response.Body.Flush();
            if (context?.Result is PhysicalFileResult fileResult)
            {
                File.Delete(fileResult.FileName);
            }
        }
    }
}
