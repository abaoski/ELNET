using System.ComponentModel.DataAnnotations;

namespace SubdivisionManagement.Models.ViewModels
{
    public class EditUserViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public UserRoleType Role { get; set; }
        
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? BlockLot { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 