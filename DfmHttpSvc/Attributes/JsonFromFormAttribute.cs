using System;
using DfmHttpSvc.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfmHttpSvc.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class JsonFromFormAttribute : ModelBinderAttribute
    {
        public JsonFromFormAttribute()
        {
            BinderType = typeof(JsonModelBinder);
        }

        public override BindingSource BindingSource => BindingSource.Form;
    }
}