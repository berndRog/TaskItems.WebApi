using System.Globalization;

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

   #region MyRegion
   public TaskItem Set(string title, string desription) {
      Id = Guid.NewGuid();
      Title = title;
      Description= desription;
      return this;
   }
   #endregion
}