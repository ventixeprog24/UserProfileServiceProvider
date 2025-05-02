

using UserProfileServiceProvider.Data.Entities;

namespace UserProfileService.Factories;

public class UserProfileFactory
{
    public static UserProfileEntity ToEntity(UserProfile request)
    {
        if (request is null)
            return null;

        UserProfileEntity userProfileEntity = new()
        {
            Id = request.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            UserAddress = new UserProfileAddressEntity
            {
                UserId = request.UserId,
                Address = request.Address,
                PostalCode = request.PostalCode,
                City = request.City
            }
        };

        return userProfileEntity;
    }

    public static UserProfileEntity UpdateEntity(UserProfile request, UserProfileEntity oldEntity)
    {
        if (request is null)
            return null;

        oldEntity.FirstName = request.FirstName;
        oldEntity.LastName = request.LastName;
        oldEntity.Email = request.Email;
        oldEntity.PhoneNumber = request.PhoneNumber;
        oldEntity.UserAddress.Address = request.Address;
        oldEntity.UserAddress.PostalCode = request.PostalCode;
        oldEntity.UserAddress.City = request.City;

        return oldEntity;
    }

    public static UserProfile ToModel(UserProfileEntity userProfileEntity)
    {
        UserProfile userProfile = new()
        {
            UserId = userProfileEntity.Id,
            FirstName = userProfileEntity.FirstName,
            LastName = userProfileEntity.LastName,
            Email = userProfileEntity.Email,
            PhoneNumber = userProfileEntity.PhoneNumber,
            Address = userProfileEntity.UserAddress.Address,
            PostalCode = userProfileEntity.UserAddress.PostalCode,
            City = userProfileEntity.UserAddress.City
        };

        return userProfile;
    }
}