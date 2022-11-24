using FluentAssertions; // https://fluentassertions.com/

/*
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.IO;
using System.Threading.Tasks;

using CarShop;
using CarShop.Controllers;
using CarShop.Persistence;
using CarShop.DomainModel.Dto;

namespace CarShopTest.Controllers;
public class OwnersControllerTest {

   private readonly CDbContext _dbContext;
   private readonly UsersController _controller;
   private readonly UserDto _user1Dto;
   private readonly UserDto _user2Dto;
   private readonly IEnumerable<UserDto> _usersDto;

   public OwnersControllerTest() {

      //--- Configuration ---------
      var configuration = new ConfigurationBuilder()
         .SetBasePath(Directory.GetCurrentDirectory())
         .AddJsonFile("appSettings.json", false)
         .Build();
      //--- Services ---------------
      IServiceCollection services = new ServiceCollection();
      // Logging
      var logging = configuration.GetSection("Logging");
      services.AddLogging(builder => {
         builder.ClearProviders();
         builder.AddConfiguration(logging);
         builder.AddDebug();
      });
      // Controllers
      services.AddScoped<UsersController>();
      // Repository
      services.AddScoped<IUsersRepositoryAsync, CUsersRepositoryAsync>();
      // Database
      var connectionString = configuration.GetSection("ConnectionStrings")["LocalDb"];
//    var connectionString = configuration.GetSection("ConnectionStrings")["SqlServer"];
      services.AddDbContext<CDbContext>(options =>
         options.UseSqlServer(connectionString)
       );
      var serviceProvider = services.BuildServiceProvider();
      //---    
      _dbContext = serviceProvider.GetRequiredService<CDbContext>();
      _dbContext.Database.EnsureDeleted();
      _dbContext.Database.EnsureCreated();

      _controller = serviceProvider.GetRequiredService<UsersController>();

      _user1Dto = new UserDto {
         Id = new Guid("10000000-0000-0000-0000-000000000000"),
         FirstName = "Achim",
         LastName = "Arndt",
         Email = "a.arndt@t-online.de",
         Phone = "0123 456 7890",
         ImagePath = @"C:\CarShp.WebApi\Images\image1.jpg"         
      };
      _user2Dto = new UserDto {
         Id = new Guid("20000000-0000-0000-0000-000000000000"),
         FirstName = "Bernhard",
         LastName = "Bauer",
         Email = "b.bauer@outlook.com",
         Phone = "0234 567 8901",
         ImagePath = @"C:\CarShp.WebApi\Images\image2.jpg"         
      };   
      _usersDto = new List<UserDto> { _user2Dto, _user2Dto };
   }

   [Fact]
   public async Task GetAsyncOkTest() {
      // Arrange
      await _controller.PostAsync(_user2Dto);
      // Act
      var response = await _controller.GetAsync();
      // Assert
      var result = THelper.ResultFromResponse<OkObjectResult, IEnumerable<Owner>>(response);
      result.StatusCode.Should().Be(200);
      (result.Value as IEnumerable<Owner>)
         .Should().NotBeNull()
         .And.NotBeEmpty()
         .And.HaveCount(2)
         .And.BeEquivalentTo(_usersDto);
   }
   [Fact]
   public async Task GetByIdAsyncOkTest() {
      // Arrange
      await _controller.PostAsync(_user2Dto);
      // Act
      var response = await _controller.GetByIdAsync(_user2Dto.Id);
      // Assert
      var result = THelper.ResultFromResponse<OkObjectResult, Owner>(response);
      result.StatusCode.Should().Be(200);
      (result.Value as Owner).Should().BeEquivalentTo<Owner>(_user2Dto);
   }
   [Fact]
   public async Task GetByIdAsyncNotFoundTest() {
      // Arrange
      await _controller.PostAsync(_user2Dto);
      var idError = new Guid("12345678-0000-0000-0000-000000000000");
      // Act
      var response = await _controller.GetByIdAsync(idError);
      // Assert
      var result = THelper.ResultFromResponse<NotFoundResult, Owner>(response);
      result!.StatusCode.Should().Be(404);
   }
   [Fact]
   public async Task PostAsyncCreatedAtTest() {
      // Arrange
      // Act
      var response = await _controller.PostAsync(_user2Dto);
      // Assert
      var result = THelper.ResultFromResponse<CreatedResult, Owner>(response);
      result!.StatusCode.Should().Be(201);
      (result!.Value as Owner).Should().BeEquivalentTo(_user2Dto);  // state equals
   }
   [Fact]
   public async Task PostAsyncConflictTest() {
      // Arrange
      // Act
      await _controller.PostAsync(_user2Dto);
      var response = await _controller.PostAsync(_user2Dto);
      // Assert
      var result = THelper.ResultFromResponse<ConflictObjectResult, Owner>(response);
      result.StatusCode.Should().Be(409);
   }
   [Fact]
   public async Task PutAsyncUpdateOkTest() {
      // Arrange
      var id = new Guid("10000000-0000-0000-0000-000000000000");
      await _controller.PostAsync(_user2Dto);
      var updatedOwnerDto = new Owner {
         Id = id,
         Name = "Erika Meier",
         Birthdate = new DateTime(1980, 2, 1),
         Email = "erika.meier@icloud.com"
      };
      // Act
      var response = await _controller.PutAsync(id, updatedOwnerDto);
      // Assert
      var result = THelper.ResultFromResponse<OkObjectResult, Owner>(response);
      result.StatusCode.Should().Be(200);
      (result.Value as Owner).Should().BeEquivalentTo(updatedOwnerDto);  // state equals
   }
   [Fact]
   public async Task PutAsyncInsertOkTest() {
      // Arrange
      var id = new Guid("10000000-0000-0000-0000-000000000000");
      var updatedOwner = new Owner {
         Id = id,
         Name = "Erika Meier",
         Birthdate = new DateTime(1980, 2, 1),
         Email = "erika.meier@icloud.com"
      };
      // Act
      var response = await _controller.PutAsync(id, updatedOwner);
      // Assert
      var result = THelper.ResultFromResponse<CreatedResult,Owner>(response);
      result.StatusCode.Should().Be(201);
      (result.Value as Owner).Should().BeEquivalentTo(updatedOwner);  // state equals
   }
   [Fact]
   public async Task PutAsyncBadRequestTest() {
      // Arrange
      var id = new Guid("10000000-0000-0000-0000-000000000000");
      await _controller.PostAsync(_user2Dto);
      var updatedOwner = new Owner {
         Id = id,
         Name = "Erika Meier",
         Birthdate = new DateTime(1980, 2, 1),
         Email = "erika.meier@icloud.com"
      };
      // Act
      var response = await _controller.PutAsync(new Guid("12345678-0000-0000-0000-000000000000"), updatedOwner);
      // Assert
      response.Should().NotBeNull();
      response.Result.Should().BeOfType<BadRequestObjectResult>();
   }

   [Fact]
   public async Task DeleteAsyncTest() {
      // Arrange
      await _controller.PostAsync(_user2Dto);
      // Act
      var response = await _controller.DeleteAsync(_user2Dto.Id);
      // Assert
      var result = THelper.ResultFromResponse<NoContentResult,Owner>(response);
      result.StatusCode.Should().Be(204); 
   }
}

*/