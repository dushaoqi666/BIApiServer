using Refit;
using System.Text.Json;
using System.Text.Json.Serialization;
using BIApiServer.Common.Converters;

namespace BIApiServer.Common
{
    public static class RefitConfig
    {
        public static RefitSettings GetDefaultSettings()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = 
                {
                    new JsonStringEnumConverter(),
                    new DateTimeConverter()
                }
            };

            return new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(options)
            };
        }
    }
} 