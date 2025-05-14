using UserProfileService.Factories;
using UserProfileServiceProvider;
using UserProfileServiceProvider.Data.Entities;

namespace Tests.UnitTests;

public class UserProfileFactory_Tests
{
    [Fact]
    public void ToEntity_ShouldReturnUserProfileEntity_WhenValidDataProvided()
    {
        //Arrange
        UserProfile userProfile = new()
        {
            UserId = "asdf",
            FirstName = "Felix",
            LastName = "Felixon",
            Email = "felix@domain.com",
            PhoneNumber = "0731234234",
            Address = "Gatan 1",
            PostalCode = "12345",
            City = "Stockholm"
        };

        //Act
        var result = UserProfileFactory.ToEntity(userProfile);

        //Assert
        Assert.NotNull(result);
        Assert.IsType<UserProfileEntity>(result);
    }
    
    [Fact]
    public void ToEntity_ShouldReturnNull_WhenNullDataProvided()
    {
        //Act
        var result = UserProfileFactory.ToEntity(null!);
        
        //Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToModel_ShouldReturnUserProfileEntity_WhenValidDataProvided()
    {
        //Arrange
        UserProfileEntity userProfileEntity = new()
        {
            Id = "userId",
            FirstName = "Felix",
            LastName = "Felixon",
            Email = "Felix@domain.com",
            PhoneNumber = "0731234234",
            UserAddress = new UserProfileAddressEntity
            {
                UserId = "userId",
                Address = "Gatan 12",
                PostalCode = "12345",
                City = "Stockholm"
            }
        };
        
        //Act
        var result = UserProfileFactory.ToModel(userProfileEntity);
        
        //Assert
        Assert.NotNull(result);
        Assert.IsType<UserProfile>(result);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_WhenNullDataProvided()
    {
        //Act
        var result = UserProfileFactory.ToModel(null!);
        
        //Assert
        Assert.Null(result);
    }
}