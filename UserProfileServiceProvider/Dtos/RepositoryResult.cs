using UserProfileServiceProvider.Data.Entities;

namespace UserProfileServiceProvider.Dtos
{
    public class RepositoryResult
    {
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public UserProfileEntity? UserProfile { get; set; }
        public IEnumerable<UserProfileEntity>? AllUserProfiles { get; set; }
    }
}
