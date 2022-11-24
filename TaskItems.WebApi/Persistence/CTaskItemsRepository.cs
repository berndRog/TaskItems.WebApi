using TaskItems.DomainModel.Entities;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace TaskItems.Persistence;
public class CTaskItemsRepository : ITaskItemsRepository {

   #region fields
   private readonly CDbContext _dbContext;
   #endregion

   #region ctor
   public CTaskItemsRepository(CDbContext dbContext) {
      _dbContext = dbContext;
      _dbContext.Database.EnsureCreated();
   }
   #endregion

   #region methods
   public async Task<TaskItem?> FindByIdAsync(Guid id) => 
      await _dbContext.TaskItems.FirstOrDefaultAsync(t => t.Id == id);

   public async Task<IEnumerable<TaskItem>> SelectAsync() =>
      await _dbContext.TaskItems.AsNoTracking().ToListAsync();

   public async Task AddAsync(TaskItem taskItem) =>
      await _dbContext.TaskItems.AddAsync(taskItem);
   
   public async Task AddRangeAsync(IEnumerable<TaskItem> taskItems) =>
      await _dbContext.AddRangeAsync(taskItems);

   public async Task UpdateAsync(TaskItem taskItem) {
      var retrievedOwner = await _dbContext.TaskItems
                                           .FirstOrDefaultAsync(t => t.Id == taskItem.Id);
      if (retrievedOwner == null)
         throw new ApplicationException($"Update, owner not found");
      _dbContext.Entry(retrievedOwner).CurrentValues.SetValues(taskItem);
      _dbContext.Entry(retrievedOwner).State = EntityState.Modified;
   }

   public async Task RemoveAsync(TaskItem taskItem) {
      var retrievedOwner = await _dbContext.TaskItems
                                           .FirstOrDefaultAsync(t => t.Id == taskItem.Id);
      if (retrievedOwner != null)
         _dbContext.TaskItems.Remove(taskItem);
   }

   public async Task<bool> SaveChangesAsync() =>
      await _dbContext.SaveAllChangesAsync();

    #endregion
}