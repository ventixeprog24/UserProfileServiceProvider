using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using UserProfileService.Factories;
using UserProfileServiceProvider;
using UserProfileServiceProvider.Data.Contexts;

namespace UserProfileService.Services;

public class ProfileService(DataContext context) : UserProfileServiceProvider.UserProfileService.UserProfileServiceBase
{
    private readonly DataContext _dbContext = context;

    public override Task<UserProfileReply> CreateUserProfile(UserProfile request, ServerCallContext context)
    {
        var userProfileEntity = UserProfileFactory.ToEntity(request);
        if (userProfileEntity is null)
            return Task.FromResult(new UserProfileReply
            {
                StatusCode = 400,
                Message = "Bad Request"
            });

        try
        {
            _dbContext.UserProfiles.Add(userProfileEntity);
            _dbContext.SaveChanges();

            return Task.FromResult(new UserProfileReply
            {
                StatusCode = 201,
                Message = "User Profile Created"
            });
        }
        catch
        {
            return Task.FromResult(new UserProfileReply
            {
                StatusCode = 500,
                Message = "Internal Server Error"
            });
        }

    }

    public override Task<RequestByUserIdReply> GetUserProfileById(RequestByUserId request, ServerCallContext context)
    {
        try
        {
            var userEntity = _dbContext.UserProfiles.Include(u => u.UserAddress).SingleOrDefault(u => u.Id == request.UserId);

            var userProfile = UserProfileFactory.ToModel(userEntity);

            return Task.FromResult(new RequestByUserIdReply
            {
                StatusCode = 200,
                Profile = userProfile,
            });
        }
        catch
        {
            return Task.FromResult(new RequestByUserIdReply
            {
                StatusCode = 404,
            });
        }

    }

    public override async Task<Profiles> GetAllUserProfiles(Empty request, ServerCallContext context)
    {
        var entities = await _dbContext.UserProfiles.Include(u => u.UserAddress).ToListAsync();

        //GPT generated solution to get the list with correct models working with the gRPC return.
        Profiles profiles = new();
        profiles.AllUserProfiles.AddRange(entities.Select(e => UserProfileFactory.ToModel(e)));

        return profiles;
    }

    public override async Task<UserProfileReply> UpdateUser(UserProfile request, ServerCallContext context)
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

        return new UserProfileReply
        {
            StatusCode = 200,
            Message = "User profile updated successfully."
        };
    }

    public override async Task<UserProfileReply> DeleteUser(RequestByUserId request, ServerCallContext context)
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

        return new UserProfileReply
        {
            StatusCode = 200,
            Message = "User successfully deleted"
        };
    }
}