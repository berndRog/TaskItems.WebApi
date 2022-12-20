using TaskItems.DomainModel.Entities;
using Microsoft.EntityFrameworkCore;
namespace TaskItems.Persistence; 

public class CDbContext: DbContext  {

   #region fields
   private readonly ILogger<CDbContext> _logger  = default!;
   #endregion

   #region properties
   public DbSet<TaskItem> TaskItems{ get; set; } = default!;
   #endregion

   #region ctor
   // Migration
   public CDbContext(DbContextOptions<CDbContext> options) : base(options) {}
   public CDbContext(DbContextOptions<CDbContext> options, ILogger<CDbContext> logger)
        : base(options) {
      _logger = logger;
   }
   #endregion

   #region methods
   public async Task<bool> SaveAllChangesAsync() {
      _logger.LogDebug("SaveChanges() {HashCode}", GetHashCode()); 
      _logger.LogDebug("\n{Tracker}",ChangeTracker.DebugView.LongView);         
      
      var result = await SaveChangesAsync();      
      
      _logger.LogDebug("SaveChanges {result}",result);
      _logger.LogDebug("\n{Tracker}",ChangeTracker.DebugView.LongView);
      return result > 0;
   }
   #endregion
}