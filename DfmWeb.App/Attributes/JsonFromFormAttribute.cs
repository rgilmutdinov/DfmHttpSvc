using System;
using DfmWeb.App.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DfmWeb.App.Attributes
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