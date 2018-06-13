using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DfmHttpSvc.Configuration
{
    public class JsonModelBinder : IModelBinder
    {
        private readonly MvcJsonOptions _options;

        public JsonModelBinder(IOptions<MvcJsonOptions> options)
        {
            this._options = options.Value;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // test if a value is received
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

                // deserialize from string
                string serialized = value.FirstValue;

                // use custom json options defined in startup if available
                object deserialized = this._options?.SerializerSettings == null ?
                    JsonConvert.DeserializeObject(serialized, bindingContext.ModelType) :
                    JsonConvert.DeserializeObject(serialized, bindingContext.ModelType, this._options.SerializerSettings);

                // set succesful binding result
                bindingContext.Result = ModelBindingResult.Success(deserialized);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
