using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Caching.Memory;
using UserProfileService.Factories;
using UserProfileServiceProvider;
using UserProfileServiceProvider.Data.Contexts;
using UserProfileServiceProvider.Data.Entities;

namespace UserProfileService.Services;

public class ProfileService(DataContext context, IMemoryCache cache) : UserProfileServiceProvider.UserProfileService.UserProfileServiceBase
{
    private readonly DataContext _dbContext = context;
    private readonly IMemoryCache _cache = cache;
    private const string _cacheKey = "Profiles";

    public override async Task<UserProfileReply> CreateUserProfile(UserProfile request, ServerCallContext context)
    {
        var userProfileEntity = UserProfileFactory.ToEntity(request);
        if (userProfileEntity is null)
            return new UserProfileReply
            {
                StatusCode = 400,
                Message = "Bad Request"
            };

        try
        {
            _dbContext.UserProfiles.Add(userProfileEntity);
            await _dbContext.SaveChangesAsync();

            _cache.Remove(_cacheKey);

            return new UserProfileReply
            {
                StatusCode = 201,
                Message = "User Profile Created"
            };
        }
        catch
        {
            return new UserProfileReply
            {
                StatusCode = 500,
                Message = "Internal Server Error"
            };
        }
    }

    public override async Task<RequestByUserIdReply> GetUserProfileById(RequestByUserId request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            return new RequestByUserIdReply
            {
                StatusCode = 400,
            };

        UserProfileEntity userProfileEntity = new();
        UserProfile userProfile = new();
        if (_cache.TryGetValue(_cacheKey, out List<UserProfileEntity>? cachedItems))
        {
            userProfileEntity = cachedItems.FirstOrDefault(u => u.Id == request.UserId);
            if (userProfileEntity is not null)
            {
                userProfile = UserProfileFactory.ToModel(userProfileEntity);
                return new RequestByUserIdReply
                {
                    StatusCode = 200,
                    Profile = userProfile
                };
            }
        }

        var userProfileList = await UpdateCacheAsync();

        userProfileEntity = userProfileList.FirstOrDefault(u => u.Id == request.UserId);
        if (userProfileEntity is null)
            return new RequestByUserIdReply
            {
                StatusCode = 404
            };

        userProfile = UserProfileFactory.ToModel(userProfileEntity);
        if (userProfile is null)
            return new RequestByUserIdReply
            {
                StatusCode = 500,
            };

        return new RequestByUserIdReply
        {
            StatusCode = 200,
            Profile = userProfile
        };
    }

    public override async Task<Profiles> GetAllUserProfiles(Empty request, ServerCallContext context)
    {
        Profiles profiles = new();

        if (_cache.TryGetValue(_cacheKey, out List<UserProfileEntity>? cachedItems))
        {
            profiles.AllUserProfiles.AddRange(cachedItems.Select(entity => UserProfileFactory.ToModel(entity)));
            return profiles;
        }

        var userProfileEntities = await UpdateCacheAsync();

        if (userProfileEntities is null)
            return profiles;

        profiles.AllUserProfiles.AddRange(userProfileEntities.Select(entity => UserProfileFactory.ToModel(entity)));

        return profiles;
    }

    public override async Task<UserProfileReply> UpdateUser(UserProfile request, ServerCallContext context)
    {
        try
        {
            var oldEntity = await _dbContext.UserProfiles.Include(u => u.UserAddress)
                .FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (oldEntity is null)
                return new UserProfileReply
                {
                    StatusCode = 404,
                    Message = "Not found."
                };

            var updatedEntity = UserProfileFactory.UpdateEntity(request, oldEntity);

            _dbContext.UserProfiles.Update(updatedEntity);
            await _dbContext.SaveChangesAsync();
        }
        catch
        {
            return new UserProfileReply
            {
                StatusCode = 500,
                Message = "Internal server error"
            };
        }

        _cache.Remove(_cacheKey);

        return new UserProfileReply
        {
            StatusCode = 200,
            Message = "User profile updated successfully."
        };
    }

    public override async Task<UserProfileReply> DeleteUser(RequestByUserId request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
            return new UserProfileReply
            {
                StatusCode = 400,
                Message = "Bad request"
            };

        try
        {
            var userToDelete = await _dbContext.UserProfiles.FirstOrDefaultAsync(u => u.Id == request.UserId);
            if (userToDelete is null)
                return new UserProfileReply
                {
                    StatusCode = 404,
                    Message = "Not found"
                };

            var userAddressToDelete =
                await _dbContext.UserProfileAddresses.FirstOrDefaultAsync(a => a.UserId == request.UserId);
            if (userAddressToDelete is null)
                return new UserProfileReply
                {
                    StatusCode = 404,
                    Message = "Not found"
                };

            _dbContext.UserProfileAddresses.Remove(userAddressToDelete);
            _dbContext.UserProfiles.Remove(userToDelete);

            await _dbContext.SaveChangesAsync();
        }
        catch
        {
            return new UserProfileReply
            {
                StatusCode = 500,
                Message = "Internal server error"
            };
        }
        

        _cache.Remove(_cacheKey);

        return new UserProfileReply
        {
            StatusCode = 200,
            Message = "User successfully deleted"
        };
    }

    public async Task<List<UserProfileEntity>?> UpdateCacheAsync()
    {
        _cache.Remove(_cacheKey);

        List<UserProfileEntity> returnList = [];

        try
        {
            returnList = await _dbContext.UserProfiles.Include(u => u.UserAddress).ToListAsync();
            if (returnList is null || returnList.Count == 0)
                return returnList;
        }
        catch
        {
            return returnList;
        }

        _cache.Set(_cacheKey, returnList, TimeSpan.FromHours(12));
        return returnList;
    }
}