using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using UserProfileService.Factories;
using UserProfileServiceProvider;
using UserProfileServiceProvider.Data.Contexts;
using UserProfileServiceProvider.Data.Entities;
using UserProfileServiceProvider.Data.Repositories;

namespace Tests.IntegrationsTests;

public class UserProfileRepository_Tests
{
    private readonly IUserProfileRepository _repository;
    private readonly DataContext _dataContext;

    public UserProfileRepository_Tests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb-{Guid.NewGuid()}").Options;
        
        _dataContext = new DataContext(options);
        _repository = new UserProfileRepository(_dataContext);
    }
    
    #region Create Tests
    [Fact]
    public async Task CreateUserProfile_ShouldReturnSucceeded_WithValidData()
    {
        //Arrange
        var userProfileEntity = new UserProfileEntity()
        {
            Id = "11111111111111111111",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@domain.com",
            PhoneNumber = "+46730789456",
            UserAddress = new UserProfileAddressEntity
            {
                UserId = "11111111111111111111",
                Address = "123 Main St",
                PostalCode = "15979",
                City = "London"
            }
        };
        
        //Act
        var result = await _repository.CreateUserProfile(userProfileEntity);
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.Succeeded);
    }
    
    [Fact]
    public async Task CreateUserProfile_ShouldReturnFalse_WithNullData()
    {
        //Act
        var result = await _repository.CreateUserProfile(null);
        
        //Assert
        Assert.NotNull(result);
        Assert.False(result.Succeeded);
    }
    #endregion
    
    #region Get Tests
    [Fact]
    public async Task GetUserByProfileId_ShouldReturnUserProfile_WithValidData()
    {
        //Arrange
        var userProfileEntity = new UserProfileEntity()
        {
            Id = "11111111111111111111",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@domain.com",
            PhoneNumber = "+46730789456",
            UserAddress = new UserProfileAddressEntity
            {
                UserId = "11111111111111111111",
                Address = "123 Main St",
                PostalCode = "15979",
                City = "London"
            }
        };
        await _repository.CreateUserProfile(userProfileEntity);
        
        //Act
        var result = await _repository.GetUserProfileById(userProfileEntity.Id);
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.Succeeded);
        Assert.NotNull(result.UserProfile);
    }

    [Theory]
    [InlineData("bogus-schmogus")]
    [InlineData(null)]
    public async Task GetUserByProfileId_ShouldReturnFalse_WithInvalidId(string? id)
    {
        //Arrange
        var userProfileEntity = new UserProfileEntity()
        {
            Id = "11111111111111111111",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@domain.com",
            PhoneNumber = "+46730789456",
            UserAddress = new UserProfileAddressEntity
            {
                UserId = "11111111111111111111",
                Address = "123 Main St",
                PostalCode = "15979",
                City = "London"
            }
        };
        await _repository.CreateUserProfile(userProfileEntity);

        //Act
        var result = await _repository.GetUserProfileById(id!);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task GetAllUserProfiles_ShouldReturnAllUserProfiles()
    {
        //Act
        var result  = await _repository.GetAllUserProfiles();
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.Succeeded);
        Assert.NotNull(result.AllUserProfiles);
    }
    #endregion
    
    #region Update Tests
    [Fact]
    public async Task UpdateUserProfile_ShouldReturnSucceeded_WithValidData()
    {
        //Arrange
        var startUserProfileEntity = new UserProfileEntity()
        {
            Id = "11111111111111111111",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@domain.com",
            PhoneNumber = "+46730789456",
            UserAddress = new UserProfileAddressEntity
            {
                UserId = "11111111111111111111",
                Address = "123 Main St",
                PostalCode = "15979",
                City = "London"
            }
        };
        
        await _repository.CreateUserProfile(startUserProfileEntity);
        var getUserProfileEntity = await _repository.GetUserProfileById(startUserProfileEntity.Id);

        var userProfile = new UserProfile()
        {
            UserId = "11111111111111111111",
            FirstName = "John",
            LastName = "Doe",
            Email = "John@domain.com",
            PhoneNumber = "+735987654",
            Address = "12 Corner St",
            PostalCode = "15979",
            City = "Westminster"
        };
        
        var updatedUserProfileEntity = UserProfileFactory.UpdateEntity(userProfile, startUserProfileEntity);
        
        //Act
        var result  = await _repository.UpdateUserProfile(updatedUserProfileEntity);
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task UpdateUserProfile_ShouldReturnFalse_WithNullData()
    {
        //Act
        var result = await _repository.UpdateUserProfile(null!);
        
        //Assert
        Assert.NotNull(result);
        Assert.False(result.Succeeded);
    }
    #endregion
    
    #region Delete Tests
    [Fact]
    public async Task DeleteUserProfile_ShouldReturnSucceeded_WithValidData()
    {
        //Arrange
        var startUserProfileEntity = new UserProfileEntity()
        {
            Id = "11111111111111111111",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@domain.com",
            PhoneNumber = "+46730789456",
            UserAddress = new UserProfileAddressEntity
            {
                UserId = "11111111111111111111",
                Address = "123 Main St",
                PostalCode = "15979",
                City = "London"
            }
        };
        
        await _repository.CreateUserProfile(startUserProfileEntity);
        
        //Act
        var result =  await _repository.DeleteUserProfile(startUserProfileEntity.Id);
        
        //Assert
        Assert.NotNull(result);
        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData("bogus-schmogus")]
    [InlineData(null)]
    public async Task DeleteUserProfile_ShouldReturnFalse_WithInvalidData(string? id)
    {
        //Act
        var result = await _repository.DeleteUserProfile(id!);
        
        //Assert
        Assert.NotNull(result);
        Assert.False(result.Succeeded);
    }
    #endregion
}