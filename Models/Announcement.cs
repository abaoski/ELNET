using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubdivisionManagement.Models
{
    public class Announcement
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Content { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public int CreatedById { get; set; }
        
        [ForeignKey("CreatedById")]
        public User? CreatedBy { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
} 