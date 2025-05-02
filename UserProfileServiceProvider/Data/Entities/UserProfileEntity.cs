using System.ComponentModel.DataAnnotations;

namespace UserProfileServiceProvider.Data.Entities;

public class UserProfileEntity
{
    [Key]
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public UserProfileAddressEntity? UserAddress { get; set; }
}