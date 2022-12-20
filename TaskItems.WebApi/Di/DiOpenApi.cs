using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace TaskItems.Di;

public static class DiOpenApi {
   public static IServiceCollection AddDiOpenApi(
      this IServiceCollection serviceCollection
   ) {
      // https://www.telerik.com/blogs/your-guide-rest-api-versioning-aspnet-core
      serviceCollection.AddApiVersioning(opt => {
         opt.DefaultApiVersion = new ApiVersion(1, 0);
         opt.AssumeDefaultVersionWhenUnspecified = true;
         opt.ReportApiVersions = true;
         //opt.ApiVersionReader =
         //   ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
         //                            new HeaderApiVersionReader("x-api-version"),
         //                            new MediaTypeApiVersionReader("x-api-version"),
         //                            new QueryStringApiVersionReader("api-version"));
      });

      // Swagger with different versions
      serviceCollection.AddVersionedApiExplorer(setup => {
         setup.GroupNameFormat = "'v'VVV";
         setup.SubstituteApiVersionInUrl = true;
      });

      // Swagger/OpenAPI
      serviceCollection.AddEndpointsApiExplorer();
      serviceCollection.AddSwaggerGen(
         options => {
            var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
            options.IncludeXmlComments(filePath);
         });

      serviceCollection.ConfigureOptions<ConfigureSwaggerOptions>();       
      return serviceCollection;
   }
}

// API Versioning
public class ConfigureSwaggerOptions: IConfigureNamedOptions<SwaggerGenOptions> {
   private readonly IApiVersionDescriptionProvider provider;

   public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) {
      this.provider = provider;
   }

   public void Configure(SwaggerGenOptions options) {
      // add swagger document for every API version discovered
      foreach (var description in provider.ApiVersionDescriptions) {
         options.SwaggerDoc(
             description.GroupName,
             CreateVersionInfo(description));
      }
   }

   public void Configure(string name, SwaggerGenOptions options) {
      Configure(options);
   }

   private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description) {
      var info = new OpenApiInfo() {
         Title = "TaskItems.WebApi",
         Description = "Rest API für Aufgabeverwaltung",
         Version = description.ApiVersion.ToString()
      };

      if (description.IsDeprecated) 
         info.Description += " This API version has been deprecated.";
      
      return info;
   }
}