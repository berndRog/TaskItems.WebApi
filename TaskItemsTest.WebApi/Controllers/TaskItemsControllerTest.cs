using FluentAssertions; // https://fluentassertions.com/

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using TaskItems.Controllers;
using TaskItems.DomainModel.Entities;
using TaskItems.Persistence;

namespace TaskItemsTest.Controllers;
public class TaskItemsControllerTest {

   private readonly CDbContext _dbContext;
   private readonly TaskItemsController _controller;

   public TaskItemsControllerTest() {

      //--- Configuration ---------
      var configuration = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appsettings.json", false)
         .Build();
      //--- Services ---------------
      IServiceCollection serviceCollection = new ServiceCollection();
      // Logging
      var logging = configuration.GetSection("Logging");
      serviceCollection.AddLogging(builder => {
         builder.ClearProviders();
         builder.AddConfiguration(logging);
         builder.AddDebug();
      });
      // Controllers
      serviceCollection.AddScoped<TaskItemsController>();
      // Database
      serviceCollection.AddPersistenceTest();
      ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider() 
         ?? throw new Exception("Failed to build Serviceprovider");

      //---    
      _dbContext = serviceProvider.GetRequiredService<CDbContext>();
      _dbContext.Database.EnsureDeleted();
      _dbContext.Database.EnsureCreated();

      _controller = serviceProvider.GetRequiredService<TaskItemsController>();      
   }

   [Fact]
   public async Task GetAsyncOkTest() {
      // Arrange
      TaskItem taskItem1 = new TaskItem().Set("Aufgabe 1","Details 1");
      TaskItem taskItem2 = new TaskItem().Set("Aufgabe 2","Details 2");
      List<TaskItem> taskItems = new List<TaskItem>() { taskItem1, taskItem2 };
      await _controller.PostAsync(taskItem1);
      await _controller.PostAsync(taskItem2);
      // Act
      var response = await _controller.GetAsync();
      // Assert
      var result = TestHelper.ResultFromResponse<OkObjectResult, IEnumerable<TaskItem>>(response);
      result.Should().NotBeNull();
      result.StatusCode.Should().Be(200);
      (result.Value as IEnumerable<TaskItem>).Should().BeEquivalentTo(taskItems);
   }

   [Fact]
   public async Task GetByIdAsyncOkTest() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      await _controller.PostAsync(taskItem);
      // Act
      var response = await _controller.GetByIdAsync(taskItem.Id);
      // Assert
      var result = TestHelper.ResultFromResponse<OkObjectResult, TaskItem>(response);      
      result.Should().NotBeNull();
      result.StatusCode.Should().Be(200);
      (result.Value as TaskItem).Should().BeEquivalentTo(taskItem);
   }
  
   [Fact]
   public async Task GetByIdAsyncNotFoundTest() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      await _controller.PostAsync(taskItem);
      var idError = new Guid("12345678-0000-0000-0000-000000000000");
      // Act
      var response = await _controller.GetByIdAsync(idError);
      // Assert
      var result = TestHelper.ResultFromResponse<NotFoundObjectResult, TaskItem>(response);
      result.Should().NotBeNull();
      result.StatusCode.Should().Be(404);
   }

   [Fact]
   public async Task PostAsyncCreatedAtTest() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      // Act
      var response = await _controller.PostAsync(taskItem);
      // Assert
      var result = TestHelper.ResultFromResponse<CreatedResult, TaskItem>(response);
      result.Should().NotBeNull();
      result.StatusCode.Should().Be(201);
      (result.Value as TaskItem).Should().BeEquivalentTo(taskItem);  // state equals
   }

   [Fact]
   public async Task PostAsyncConflictTest() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      await _controller.PostAsync(taskItem);
      // Act
      var response = await _controller.PostAsync(taskItem);
      // Assert
      var result = TestHelper.ResultFromResponse<ConflictObjectResult, TaskItem>(response);
      result.Should().NotBeNull();
      result.StatusCode.Should().Be(409);
   }

   [Fact]
   public async Task PutAsyncUpdateOkTest() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      await _controller.PostAsync(taskItem);
      // Act
      taskItem.Title = "Task 1";
      taskItem.Description = "Details for task 1";
      var response = await _controller.PutAsync(taskItem.Id, taskItem);
      // Assert
      var result = TestHelper.ResultFromResponse<OkObjectResult, TaskItem>(response);
      result.Should().NotBeNull();
      result.StatusCode.Should().Be(200);
      (result.Value as TaskItem).Should().BeEquivalentTo(taskItem);
   }

   [Fact]
   public async Task PutAsyncBadRequestTest() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      await _controller.PostAsync(taskItem);
      var id = new Guid("10000000-0000-0000-0000-000000000000");     
      // Act
      var response = await _controller.PutAsync(id, taskItem);
      // Assert
      var result = TestHelper.ResultFromResponse<BadRequestObjectResult, TaskItem>(response);
      response.Should().NotBeNull();
      result.StatusCode.Should().Be(400);      
   }

      [Fact]
   public async Task PutAsyncNotFoundTest() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      await _controller.PostAsync(taskItem);
      var id = new Guid("10000000-0000-0000-0000-000000000000");     
      // Act
      var response = await _controller.PutAsync(id, taskItem);
      // Assert
      var result = TestHelper.ResultFromResponse<BadRequestObjectResult, TaskItem>(response);
      response.Should().NotBeNull();
      result.StatusCode.Should().Be(400);      
   }

}
/*
   [Fact]
   public async Task DeleteAsyncTest() {
      // Arrange
      await _controller.PostAsync(_user2Dto);
      // Act
      var response = await _controller.RemoveAsync(_user2Dto.Id);
      // Assert
      var result = THelper.ResultFromResponse<NoContentResult,Owner>(response);
      result.StatusCode.Should().Be(204); 
   }
}

*/