using System.Collections.Generic;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DfmHttpSvc.Configuration.Swagger
{
    public class SwaggerLowercaseRouteFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            IDictionary<string, PathItem> originalPaths = swaggerDoc.Paths;

            // generate the new keys
            Dictionary<string, PathItem> newPaths = new Dictionary<string, PathItem>();
            List<string> removeKeys = new List<string>();
            foreach (KeyValuePair<string, PathItem> path in originalPaths)
            {
                string newKey = LowercaseEverythingButParameters(path.Key);
                if (newKey != path.Key)
                {
                    removeKeys.Add(path.Key);
                    newPaths.Add(newKey, path.Value);
                }
            }

            // add the new keys
            foreach (KeyValuePair<string, PathItem> path in newPaths)
            {
                swaggerDoc.Paths.Add(path.Key, path.Value);
            }

            // remove the old keys
            foreach (string key in removeKeys)
            {
                swaggerDoc.Paths.Remove(key);
            }
        }

        private static string LowercaseEverythingButParameters(string key)
        {
            return string.Join(
                "/",
                key.Split('/').Select(x => x.Contains("{") ? x : x.ToLower())
            );
        }
    }
}
