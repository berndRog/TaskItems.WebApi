using Microsoft.AspNetCore.Mvc.ApiExplorer;
using TaskItems.Di;
namespace TaskItems;

public class Programm {

   static void Main(string[] args) {

      var builder = WebApplication.CreateBuilder(args);
      // Add Logging
      ConfigureLogging(builder);
      // Add services (dependency injection)
      ConfigureServices(builder);
      var app = builder.Build();
     
      //
      // Configure request pipeline.
      //
      IApiVersionDescriptionProvider provider = 
         app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
      if (app.Environment.IsDevelopment()) {        
         app.UseSwagger();         
         app.UseSwaggerUI(options => {
         foreach (var description in provider.ApiVersionDescriptions) { 
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
         }
         });
      }

      app.UseRouting();
      //      app.UseAuthentication();
      //      app.UseAuthorization();      
      app.UseEndpoints(endpoints => { 
         endpoints.MapControllers();
      });

      app.Run();
   }


   static void ConfigureLogging(WebApplicationBuilder builder) {
      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();
//    builder.Logging.AddEventLog();
      builder.Logging.AddEventSourceLogger();
      // Write Logging to Debug into a file
      // Windows C:\users\<username>\appdata\local
      // Mac       /users/<username>\.local/share
      // var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); 
      // var tracePath = Path.Join(path, $"Log_WebApi_{DateTime.Now.ToString("yyyy_MM_dd-HHmmss")}.txt");
      // Trace.Listeners.Add(new TextWriterTraceListener(File.Create(tracePath)));
      // Trace.AutoFlush = true;
   }

   static void ConfigureServices(WebApplicationBuilder builder) {
      builder.Services.AddDiController();
      builder.Services.AddDiOpenApi();
      builder.Services.AddDiPersistence(builder.Configuration);
   }
}