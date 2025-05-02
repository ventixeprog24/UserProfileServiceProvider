using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserProfileServiceProvider.Data.Entities;

public class UserProfileAddressEntity
{
    [Key, ForeignKey(nameof(UserProfile))]
    public string UserId { get; set; } = null!;
    public UserProfileEntity UserProfile { get; set; } = null!;
    public string? Address { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
}