using System.ComponentModel.DataAnnotations;

namespace SubdivisionManagement.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRoleType Role { get; set; }

        public string? PhoneNumber { get; set; }
        
        public string? Address { get; set; }
        
        public string? BlockLot { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }
    }

    public enum UserRoleType
    {
        Admin = 1,
        Homeowner = 2,
        Staff = 3,
        Tenant = 4
    }
} 