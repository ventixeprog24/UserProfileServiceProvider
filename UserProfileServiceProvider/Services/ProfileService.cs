using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using UserProfileService.Factories;
using UserProfileServiceProvider;
using UserProfileServiceProvider.Data.Entities;
using UserProfileServiceProvider.Data.Repositories;

namespace UserProfileService.Services;

public class ProfileService(IUserProfileRepository repository, IMemoryCache cache) : UserProfileServiceProvider.UserProfileService.UserProfileServiceBase
{
    private readonly IUserProfileRepository _repository = repository;
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

        var result = await _repository.CreateUserProfile(userProfileEntity);
        if (result is null || !result.Succeeded)
            return new UserProfileReply
            {
                StatusCode = 500,
                Message = "Internal Server Error"
            };

        _cache.Remove(_cacheKey);

        return new UserProfileReply
        {
            StatusCode = 201,
            Message = "User Profile Created"
        };
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
        var oldEntityResult = await _repository.GetUserProfileById(request.UserId);
        if (oldEntityResult is null || !oldEntityResult.Succeeded)
            return new UserProfileReply
            {
                StatusCode = 404,
                Message = "Not found."
            };

        var updatedEntity = UserProfileFactory.UpdateEntity(request, oldEntityResult.UserProfile!);

        var updateResult = await _repository.UpdateUserProfile(updatedEntity);
        if (updateResult is null || !updateResult.Succeeded)
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


        var deleteResult = await _repository.DeleteUserProfile(request.UserId);
        if (deleteResult is null || !deleteResult.Succeeded)
            return new UserProfileReply
            {
                StatusCode = 500,
                Message = "Not found"
            };

        _cache.Remove(_cacheKey);

        return new UserProfileReply
        {
            StatusCode = 200,
            Message = "User successfully deleted"
        };
    }

    public async Task<IEnumerable<UserProfileEntity>?> UpdateCacheAsync()
    {
        _cache.Remove(_cacheKey);

        IEnumerable<UserProfileEntity> returnList = [];

        var returnResult = await _repository.GetAllUserProfiles();
        if (returnResult is null || !returnResult.Succeeded)
            return returnList;

        returnList = returnResult.AllUserProfiles!;

        _cache.Set(_cacheKey, returnList, TimeSpan.FromHours(12));
        return returnList;
    }
}