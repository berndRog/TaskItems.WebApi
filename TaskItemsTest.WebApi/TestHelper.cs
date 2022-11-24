
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskItems.Di;
using TaskItems.DomainModel.Entities;

namespace TaskItemsTest; 

public static class TestHelper {
 
   public static IServiceCollection AddPersistenceTest(
      this IServiceCollection serviceCollection
   ) {

      // Configuration
      // Nuget:  Microsoft.Extensions.Configuration
      //       + Microsoft.Extensions.Configuration.Json
      var configuration = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json", false)
         .Build();
      serviceCollection.AddSingleton<IConfiguration>(configuration);

      // Logging
      // Nuget:  Microsoft.Extensions.Logging
      //       + Microsoft.Extensions.Logging.Configuration
      //       + Microsoft.Extensions.Logging.Debug
      var logging = configuration.GetSection("Logging");
      serviceCollection.AddLogging(builder => {
         builder.ClearProviders();
         builder.AddConfiguration(logging);
         builder.AddDebug();
      });
      
      // CarShop.Di: Repository + Database
      serviceCollection.AddDiPersistence(configuration);

      return serviceCollection;
   }



   public static OkObjectResult ResultFromResponseOwner<OkObjectResult>(
      ActionResult<IEnumerable<TaskItem>> response
   ) where OkObjectResult : class{
      response.Should().NotBeNull();
      response.Result.Should().BeOfType<OkObjectResult>();
      var result = response.Result as OkObjectResult;
      result.Should().NotBeNull();
      return result!;
   }

   public static T ResultFromResponse<T, S>(
      ActionResult<S> response
   ) where T : class where S : class{
      response.Should().NotBeNull();
      response.Result.Should().NotBeNull();
      response.Result.Should().BeOfType<T>();
      var result = response.Result as T;
      result.Should().NotBeNull();
      T ret = result!;
      return ret;
   }
}