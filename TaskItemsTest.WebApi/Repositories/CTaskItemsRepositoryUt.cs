using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

using TaskItems;
using TaskItems.DomainModel.Entities;
using TaskItems.Persistence;

namespace TaskItemsTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public class CTaskItemsRepositoryUt {
   private readonly ITaskItemsRepository _repository;   

   public CTaskItemsRepositoryUt(){
      IServiceCollection serviceCollection = new ServiceCollection();
      serviceCollection.AddPersistenceTest();
      ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider() 
         ?? throw new Exception("Failed to build Serviceprovider");
      
      var dbContext = serviceProvider.GetRequiredService<CDbContext>();
      dbContext.Database.EnsureDeleted();
      dbContext.Database.EnsureCreated();

      _repository = serviceProvider.GetRequiredService<ITaskItemsRepository>();      
   }

   [Fact]
   public async void FindByIdUt() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      await _repository.AddAsync(taskItem);
      await _repository.SaveChangesAsync();  
      // Act
      var actual = await _repository.FindByIdAsync(taskItem.Id);
      // Assert
      actual.Should().BeEquivalentTo(taskItem);
   }

   [Fact]
   public async void SelectAllUt() {
      // Arrange
      List<TaskItem> taskItems = new List<TaskItem>();
      taskItems.Add(new TaskItem().Set("Aufgabe 1","Details 1"));
      taskItems.Add(new TaskItem().Set("Aufgabe 2","Details 2"));
      await _repository.AddRangeAsync(taskItems);
      await _repository.SaveChangesAsync();
      // Act
      var actual = await _repository.SelectAsync();
      // Assert
      actual.Count().Should().Be(2);
      actual.Should().BeEquivalentTo(taskItems);
   } 

   [Fact]
   public async void AddUt() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      // Act
      await _repository.AddAsync(taskItem);
      await _repository.SaveChangesAsync();
      // Assert
      var actual = await _repository.FindByIdAsync(taskItem.Id);
      actual.Should().BeEquivalentTo(taskItem);
   }

   [Fact]
   public async void UpdateUt() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      await _repository.AddAsync(taskItem);
      await _repository.SaveChangesAsync();
      // Act
      taskItem.Title = "Task 1";
      taskItem.Description = "Details for task 1";
      await _repository.UpdateAsync(taskItem);
      await _repository.SaveChangesAsync();
      // Assert
      var actual = await _repository.FindByIdAsync(taskItem.Id);
      actual.Should().BeEquivalentTo(taskItem);
   }

   [Fact]
   public async void DeleteUt() {
      // Arrange
      TaskItem taskItem = new TaskItem().Set("Aufgabe 1", "Details zu Aufgabe 1");
      await _repository.AddAsync(taskItem);
      await _repository.SaveChangesAsync();
      // Act      
      await _repository.RemoveAsync(taskItem);
      await _repository.SaveChangesAsync();
      // Assert
      var actual = await _repository.FindByIdAsync(taskItem.Id);
      actual.Should().BeNull();
   }
}