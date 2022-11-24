using TaskItems.DomainModel.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace TaskItems.Controllers; 
[Route("carshop/")]
[ApiController]
public class TaskItemsController : ControllerBase {

   #region fields
   private readonly ITaskItemsRepository _repository;
   private readonly ILogger<TaskItemsController> _logger;
   #endregion

   #region ctor
   public TaskItemsController(
      ITaskItemsRepository repository, 
      ILogger<TaskItemsController> logger
   ) {
      _repository = repository;
      _logger = logger;
   }
   #endregion

   #region methods
   /// <summary>
   /// Find the taskItem with the given id 
   /// </summary>
   /// <param name="id:Guid">id of the taskItem</param>
   /// <returns>OwnerDto?</returns>
   /// <response code="200">Ok: OwnerDto with given id returned</response>
   /// <response code="404">NotFound: OwnerDto with given id not found</response>
   /// <response code="500">Server internal error.</response>
   [HttpGet("{id:Guid}")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status404NotFound)]
   [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<TaskItem>> GetByIdAsync(
      [FromRoute] Guid id
   ) {
      try {
         _logger.LogDebug("Get {id}",id);
         var taskItem = await _repository.FindByIdAsync(id);
         if(taskItem == null) return NotFound("Task with given id not found");
         return Ok(taskItem); 
      }
      catch (Exception e) {
         return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
      }    
   }

   /// <summary>
   /// Get all users 
   /// </summary>
   /// <returns>IEnumerable{TaskItem}; </returns>
   /// <response code="200">Ok. TaskItem returned</response>
   /// <response code="500">Server internal error.</response>
   [HttpGet("")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<IEnumerable<TaskItem>>> GetAsync() {
      try {
         _logger.LogDebug("GetAll()");
         var taskItems = await _repository.SelectAsync();
         return Ok(taskItems);
      }
      catch (Exception e) {
         return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
      }
   }
  
   /// <summary>
   /// Insert an taskItem. User has an Id, otherwise it will be created
   /// </summary>
   /// <param name="taskItem"></param>
   /// <returns>TaskItem?</returns>
   /// <response code="201">Created: TaskItem is created</response>
   /// <response code="409">Conflict: TaskItem with given id already exists.</response>
   /// <response code="500">Server internal error.</response>
   [HttpPost("")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
   [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<TaskItem>> PostAsync(
      [FromBody] TaskItem taskItem
   ) {
      try {
         _logger.LogDebug("Post");
                     
         if(await _repository.FindByIdAsync(taskItem.Id) != null) 
            return Conflict($"Post: User with given id already exists");

         // save to repository and write to database 
         await _repository.AddAsync(taskItem);
         await _repository.SaveChangesAsync();
      
         // https://ochzhen.com/blog/created-createdataction-createdatroute-methods-explained-aspnet-core
         // Request == null in unit tests
         var path = Request == null 
            ? $"/rest/tasks/{taskItem.Id}" 
            : $"{Request.Path}/{taskItem.Id}";
         var uri = new Uri(path, UriKind.Relative);
         return Created(uri: uri, value: taskItem);
    }
      catch (Exception e) {
         return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
      }
   }

   
   /// <summary>
   /// Update an taskItem, if taskItem with id exists.
   /// </summary>
   /// <param name="id:Guid">given id</param>
   /// <param name="updTaskItem">taskItem with new properties</param>
   /// <returns>Owner?</returns>
   /// <response code="200">Ok: Owner with given id updated.</response>
   /// <response code="400">Bad Request: id and Owner.Id do not match.</response>
   /// <response code="500">Server internal error.</response>
   [HttpPut("{id:Guid}")]
   [Consumes(MediaTypeNames.Application.Json)]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status200OK)]
   [ProducesResponseType(StatusCodes.Status201Created)]
   [ProducesResponseType(StatusCodes.Status400BadRequest)]
   [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<TaskItem>> PutAsync(
      [FromRoute] Guid id,
      [FromBody]  TaskItem updTaskItem
   ) {
      _logger.LogDebug("Put {id}",id);
      try {
         if(id != updTaskItem.Id) return BadRequest($"Put: Id and taskItem.Id does not match");
         var taskItem = await _repository.FindByIdAsync(id);
         if(taskItem == null) return NotFound($"Delete: TaskItem with given id not found");

         // Update taskItem
         await _repository.UpdateAsync(updTaskItem);
         await _repository.SaveChangesAsync();
         return Ok(updTaskItem);
      }
      catch (Exception e) {
         return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
      }
   }

   /// <summary>
   /// Delete the taskItem
   /// </summary>
   /// <param name="id"></param>
   /// <returns></returns>
   /// <response code="204">NoContent: Owner deleted.</response>
   /// <response code="404">NotFound: Owner with given id not found</response>
   /// <response code="500">Server internal error.</response>
   [HttpDelete("{id:Guid}")]
   [Produces(MediaTypeNames.Application.Json)]
   [ProducesResponseType(StatusCodes.Status204NoContent)]
   [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
   [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
   [ProducesDefaultResponseType]
   public async Task<ActionResult<TaskItem>> DeleteAsync(
      [FromRoute] Guid id
   ) {
      _logger.LogDebug("Delete {id}",id);
      try {
         var taskItem = await _repository.FindByIdAsync(id);
         if(taskItem == null) return NotFound($"Delete: TaskItem with given id not found");
         await _repository.RemoveAsync(taskItem);
         await _repository.SaveChangesAsync();
         return NoContent();  // 204 = Ok with no content
      }
      catch (Exception e) {
         return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
      }
   }
   #endregion
}