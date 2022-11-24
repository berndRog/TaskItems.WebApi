using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskItems.Persistence;
public class CDbContextFactory : IDesignTimeDbContextFactory<CDbContext> {

   public CDbContext CreateDbContext(string[] args) {
      // Nuget:  Microsoft.Extensions.Configuration
      //       + Microsoft.Extensions.Configuration.Json
      var configuration = new ConfigurationBuilder()
                         .SetBasePath(Directory.GetCurrentDirectory())
                         .AddJsonFile("appSettingsMigrations.json", false)
                         .Build();

      var (useDatabase, dataSource) = Helper.CreateDataSource(configuration);

      var optionsBuilder = new DbContextOptionsBuilder<CDbContext>();
      switch (useDatabase) {
         case "LocalDb":
         case "SqlServer":
            optionsBuilder.UseSqlServer(dataSource);
            break;
         case "Sqlite":
            optionsBuilder.UseSqlite(dataSource);
            break;
         default:
            throw new Exception($"appsettings.json UseDatabase {useDatabase} not available");
      }
      return new CDbContext(optionsBuilder.Options);
   }
}