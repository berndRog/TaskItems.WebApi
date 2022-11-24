namespace TaskItems.DomainModel.Entities;

public class TaskItem {

   #region properties
   public Guid Id{ get; set; } = Guid.Empty;
   public string Title{ get; set; } = string.Empty;
   public string Description{ get; set; } = string.Empty;  
   #endregion

   #region ctor
   public TaskItem() { }
   #endregion

   #region methods
   public TaskItem Set(string title, string description) {
      Id = Guid.NewGuid(); 
      Title = title;
      Description = description;      
      return this;
   }   
   public TaskItem Set(Guid id, string title, string description) {
      Id = (id == Guid.Empty) ? Guid.NewGuid() : id; 
      Title = title;
      Description = description;      
      return this;
   }   
   #endregion
}