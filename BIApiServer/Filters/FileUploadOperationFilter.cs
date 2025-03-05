using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BIApiServer.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileUploadMime = "multipart/form-data";
            if (operation.RequestBody == null ||
                !operation.RequestBody.Content.Any(x => 
                    x.Key.Equals(fileUploadMime, StringComparison.InvariantCultureIgnoreCase)))
                return;

            operation.RequestBody.Content[fileUploadMime].Schema.Properties = 
                new Dictionary<string, OpenApiSchema>
                {
                    ["file"] = new OpenApiSchema()
                    {
                        Type = "string",
                        Format = "binary"
                    }
                };
        }
    }
}