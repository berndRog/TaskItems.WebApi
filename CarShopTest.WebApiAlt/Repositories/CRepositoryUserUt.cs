using AutoMapper;

using CarShop;
using CarShop.Di;
using CarShop.DomainModel.Entities;
using CarShop.Persistence;

using CarShopTest.DomainModel.Utilities;

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CarShopTest.Persistence.Repositories;
[Collection(nameof(SystemTestCollectionDefinition))]
public class CRepositoryUserUt {

   private readonly IUsersRepositoryAsync _repository;   
   private readonly IMapper _mapper;

   public CRepositoryUserUt(){
      IServiceCollection serviceCollection = new ServiceCollection();
      serviceCollection.AddPersistenceTest();

      serviceCollection.AddAutoMapper(typeof(User), typeof(MappingProfile));
      // Auto Mapper Configurations
      var mapperConfig = new MapperConfiguration(mc => {
         mc.AddProfile(new MappingProfile());
      });

      ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider() 
         ?? throw new Exception("Failed to build Serviceprovider");
      
      var dbContext = serviceProvider.GetRequiredService<CDbContext>();
      dbContext.Database.EnsureDeleted();
      dbContext.Database.EnsureCreated();

      _repository = serviceProvider.GetRequiredService<IUsersRepositoryAsync>();
      _mapper = serviceProvider.GetRequiredService<IMapper>();
   }

   #region User
   [Fact]
   public async void FindByIdUt() {
      // Arrange
      var seed = new CSeed(_mapper);
      await _repository.AddAsync(seed.User1);
      await _repository.SaveChangesAsync();  
      // Act
      var actual = await _repository.FindByIdAsync(seed.User1.Id);
      // Assert
      actual.Should().BeEquivalentTo(seed.User1);
   }

/*
   [Fact]
   public void FindByPredicateUt() {
      // Arrange
      var seed = new CSeed();
      _unitOfWork.RepositoryUser.AddRange(seed.Users);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Act
      var actual = _unitOfWork.RepositoryUser.Find(u => u.UserName == "FFischer");
      // Assert
      actual.Should().BeEquivalentTo(seed.User6);
      _unitOfWork.Dispose();
   }
   [Fact]
   public void SelectByPredicateUt() {
      // Arrange
      var seed = new CSeed();
      var expected = seed.Users.Where(u => u.Email.Contains("google.com"))
                               .ToList();
      _unitOfWork.RepositoryUser.AddRange(seed.Users);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Act
      var actual = _unitOfWork.RepositoryUser
                              .Select(u => u.Email.Contains("google.com"));
      // Assert
      actual.Count().Should().Be(2);
      actual.Should().BeEquivalentTo(expected);
      _unitOfWork.Dispose();
   }
   [Fact]
   public void SelectAllUt() {
      // Arrange
      var seed = new CSeed();
      _unitOfWork.RepositoryUser.AddRange(seed.Users);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Act
      var actual = _unitOfWork.RepositoryUser.SelectAll();
      // Assert
      actual.Count().Should().Be(6);
      actual.Should().BeEquivalentTo(seed.Users);
      _unitOfWork.Dispose();
   }   
   [Fact]
   public void AddUt() {
      // Arrange
      var seed = new CSeed();
      // Act
      _unitOfWork.RepositoryUser.Add(seed.User1);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Assert
      var actual = _unitOfWork.RepositoryUser.FindById(seed.User1.Id);
      actual.Should().BeEquivalentTo(seed.User1);
      _unitOfWork.Dispose();
   }
   [Fact]
   public void UpdateUt() {
      // Arrange
      var seed = new CSeed();
      _unitOfWork.RepositoryUser.Add(seed.User1);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Act
      seed.User1.Email = "achim.arndt@gmx.de";
      _unitOfWork.RepositoryUser.Update(seed.User1);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();         
      // Assert
      var actual = _unitOfWork.RepositoryUser.FindById(seed.User1.Id);
      actual.Should().BeEquivalentTo(seed.User1);
   }
   [Fact]
   public void DeleteUt() {
      // Arrange
      var seed = new CSeed();
      _unitOfWork.RepositoryUser.Add(seed.User1);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Act
      _unitOfWork.RepositoryUser.Remove(seed.User1);
      _unitOfWork.SaveAllChanges();
      // Assert
      var actual = _unitOfWork.RepositoryUser.FindById(seed.User1.Id);
      actual.Should().BeNull();
   }
   #endregion  

   #region User with Address
   [Fact]
   public void User_InsertUserAndUpdateUserWithAddress1Ut() {
      // Arrange
      var seed = new CSeed();
      _unitOfWork.RepositoryUser.Add(seed.User1);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Act
      User user = _unitOfWork.RepositoryUser.FindById(seed.User1.Id) ??  
         throw new Exception("user should not be null");
      user.AddOrUpdateAddress(seed.Address1);
      _unitOfWork.RepositoryUser.Update(user);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Assert
      var actual = _unitOfWork.RepositoryUser.FindById(seed.User1.Id, false);  // without join
      var actual2 = _unitOfWork.RepositoryUser.FindById(seed.User1.Id, true);  // with join
                                                // ignore cyclic references via Adress -> User -> Address ...
      actual2.Should().BeEquivalentTo(user, opt => opt.IgnoringCyclicReferences()
                                                      .Excluding(u => u.Address));
   }
   [Fact]
   public void User_InsertUserWithAddressUt() {
      // Arrange
      var seed = new CSeed();
      // Act
      seed.User1.AddOrUpdateAddress(seed.Address1);
      _unitOfWork.RepositoryUser.Add(seed.User1);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Assert
      var actual = _unitOfWork.RepositoryUser.FindById(seed.User1.Id, false);  // without join
      var actual2 = _unitOfWork.RepositoryUser.FindById(seed.User1.Id, true);  // with join
                                                // ignore cyclic references via Adress -> User -> Address ...
      actual2.Should().BeEquivalentTo(seed.User1, opt => opt.IgnoringCyclicReferences()
                                                            .Excluding(u => u.Address));
   }
   [Fact]
   public void User_InsertUserWithAddressAndUpdateAddressUt() {
      // Arrange
      var seed = new CSeed();
      seed.User1.AddOrUpdateAddress(seed.Address1);
      _unitOfWork.RepositoryUser.Add(seed.User1);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Act
      User user = _unitOfWork.RepositoryUser.FindById(seed.User1.Id, true) ??  
         throw new Exception("user should not be null");
      AddressDto address = user.Address ??  
         throw new Exception("address should not be null");
      address.Street = "Hannoversche Str";
      address.Number = "1a";      
      seed.User1.AddOrUpdateAddress(address);
      _unitOfWork.RepositoryUser.Update(user);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Assert
      var actual = _unitOfWork.RepositoryUser.FindById(user.Id, true);
      actual.Should().BeEquivalentTo(user, opt => opt.IgnoringCyclicReferences()
                                                     .Excluding(u => u.Address));
   }
   [Fact]
   public void User_RemoveWithAddressUt() {
      // Arrange
      var seed = new CSeed();
      seed.User1.AddOrUpdateAddress(seed.Address1);
      _unitOfWork.RepositoryUser.Add(seed.User1);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Act
      User user = _unitOfWork.RepositoryUser.FindById(seed.User1.Id, true) ??  
         throw new Exception("user should not be null");
      _unitOfWork.RepositoryUser.Remove(user);
      _unitOfWork.SaveAllChanges();
      _unitOfWork.Dispose();
      // Assert
      var actual = _unitOfWork.RepositoryUser.FindById(user.Id, true);
      actual.Should().BeNull();                              
   }
*/
   #endregion
}