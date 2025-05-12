using UserProfileServiceProvider.Data.Entities;
using UserProfileServiceProvider.Dtos;

namespace UserProfileServiceProvider.Data.Repositories;

public interface IUserProfileRepository
{
    Task<RepositoryResult> CreateUserProfile(UserProfileEntity request);
    Task<RepositoryResult> GetUserProfileById(string id);
    Task<RepositoryResult> GetAllUserProfiles();
    Task<RepositoryResult> UpdateUserProfile(UserProfileEntity userProfile);
    Task<RepositoryResult> DeleteUserProfile(string id);
}