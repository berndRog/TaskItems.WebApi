using TaskItems.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TaskItems.Di;

public static class DiPersistence {
   public static IServiceCollection AddDiPersistence(
      this IServiceCollection serviceCollection,
      IConfiguration          configuration
   ){
      serviceCollection.AddScoped<ITaskItemsRepository, CTaskItemsRepository>();

      var (useDatabase, dataSource) = Helper.CreateDataSource(configuration);

      switch(useDatabase){
         case "LocalDb":
         case "SqlServer":
            serviceCollection.AddDbContext<CDbContext>(o => o.UseSqlServer(dataSource));
            break;
         case "Sqlite":
            serviceCollection.AddDbContext<CDbContext>(o => o.UseSqlite(dataSource));
            break;
         default:
            throw new Exception($"appsettings.json UseDatabase {useDatabase} not available");
      }

      return serviceCollection;
   }
}