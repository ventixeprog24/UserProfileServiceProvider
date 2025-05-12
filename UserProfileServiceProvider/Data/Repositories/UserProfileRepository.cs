using Azure.Core;
using Microsoft.EntityFrameworkCore;
using UserProfileServiceProvider.Data.Contexts;
using UserProfileServiceProvider.Data.Entities;
using UserProfileServiceProvider.Dtos;

namespace UserProfileServiceProvider.Data.Repositories
{
    public class UserProfileRepository(DataContext context) : IUserProfileRepository
    {
        private readonly DataContext _dbContext = context;

        public async Task<RepositoryResult> CreateUserProfile(UserProfileEntity request)
        {
            try
            {
                _dbContext.UserProfiles.Add(request);
                await _dbContext.SaveChangesAsync();

                return new RepositoryResult { Succeeded = true };
            }
            catch (Exception ex)
            {
                return new RepositoryResult { Succeeded = false, Message = ex.Message};
            }
        }

        public async Task<RepositoryResult> GetUserProfileById(string id)
        {
            try
            {
                var returnEntity = await _dbContext.UserProfiles.Include(u => u.UserAddress)
                    .FirstOrDefaultAsync(u => u.Id == id);
                return returnEntity is null
                    ? new RepositoryResult { Succeeded = false, Message = "Not found" }
                    : new RepositoryResult { Succeeded = true, UserProfile = returnEntity };
            }
            catch (Exception ex)
            {
                return new RepositoryResult { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResult> GetAllUserProfiles()
        {
            try
            {
                IEnumerable<UserProfileEntity> returnList =
                    await _dbContext.UserProfiles.Include(u => u.UserAddress).ToListAsync();
                return returnList is null
                    ? new RepositoryResult { Succeeded = false }
                    : new RepositoryResult { Succeeded = true, AllUserProfiles = returnList };
            }
            catch (Exception ex)
            {
                return new RepositoryResult { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResult> UpdateUserProfile(UserProfileEntity userProfile)
        {
            try
            {
                _dbContext.UserProfiles.Update(userProfile);
                await _dbContext.SaveChangesAsync();
                return new RepositoryResult { Succeeded = true };
            }
            catch (Exception ex)
            {
                return new RepositoryResult { Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<RepositoryResult> DeleteUserProfile(string id)
        {
            try
            {
                var userToDelete = await _dbContext.UserProfiles.FirstOrDefaultAsync(u => u.Id == id);
                if (userToDelete is null)
                    return new RepositoryResult { Succeeded = false, Message = "User Profile not found" };

                var userAddressToDelete =
                    await _dbContext.UserProfileAddresses.FirstOrDefaultAsync(a => a.UserId == id);
                if (userAddressToDelete is null)
                    return new RepositoryResult { Succeeded = false, Message = "User Address not found" };

                _dbContext.UserProfileAddresses.Remove(userAddressToDelete);
                _dbContext.UserProfiles.Remove(userToDelete);

                await _dbContext.SaveChangesAsync();
                return new RepositoryResult { Succeeded = true };
            }
            catch (Exception ex)
            {
                return new RepositoryResult { Succeeded = false, Message = ex.Message };
            }
        }


    }
}
