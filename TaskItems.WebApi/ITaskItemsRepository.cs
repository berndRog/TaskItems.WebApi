using TaskItems.DomainModel.Entities;
namespace TaskItems;

public interface ITaskItemsRepository {
   Task<TaskItem?> FindByIdAsync(Guid id);
   Task<IEnumerable<TaskItem>> SelectAsync();
   Task AddAsync(TaskItem taskItem);
   Task AddRangeAsync(IEnumerable<TaskItem> taskItem);
   Task UpdateAsync(TaskItem taskItem);
   Task RemoveAsync(TaskItem taskItem);
   Task RemoveAllAsync();
   Task<bool> SaveChangesAsync();
}
