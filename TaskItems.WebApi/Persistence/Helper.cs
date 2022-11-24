using Microsoft.Extensions.Configuration;

namespace TaskItems.Persistence;

public static class Helper {
   public static (string useDatabase, string dataSource) CreateDataSource(IConfiguration configuration){
      //// Nuget:  Microsoft.Extensions.Configuration
      ////       + Microsoft.Extensions.Configuration.Json
      //var configuration = new ConfigurationBuilder()
      //                   .SetBasePath(Directory.GetCurrentDirectory())
      //                   .AddJsonFile("appSettings.json", false)
      //                   .Build();

      var useDatabase      = configuration.GetSection("UseDatabase").Value;
      var connectionString = configuration.GetSection("ConnectionStrings")[useDatabase];
      var directory        = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

      switch(useDatabase){
         case "LocalDb":
            var dbFile = $"{Path.Combine(directory, connectionString)}.mdf";
            var dataSourceLocalDb = $"Data Source = (LocalDB)\\MSSQLLocalDB; " +
                                    $"Initial Catalog = {connectionString}; Integrated Security = True; " +
                                    $"AttachDbFileName = {dbFile};";
            return ( useDatabase, dataSourceLocalDb );
         case "SqlServer":
            return ( useDatabase, connectionString );
         case "Sqlite":
            var dataSourceSqlite =
               "Data Source=" + Path.Combine(directory, connectionString) + ".db";
            return ( useDatabase, dataSourceSqlite );
         default:
            throw new Exception($"appsettings.json UseDatabase {useDatabase} not available");
      }
   }
}