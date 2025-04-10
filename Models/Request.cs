using System;
using System.ComponentModel.DataAnnotations;

namespace SubdivisionManagement.Models
{
    public class Request
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public RequestType Type { get; set; }
        
        public RequestStatus Status { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public string AdminNotes { get; set; }
        
        public int UserId { get; set; }
        
        public User User { get; set; }
    }
    
    public enum RequestType
    {
        Maintenance,
        Complaint,
        Permission,
        Other
    }
    
    public enum RequestStatus
    {
        Pending,
        Approved,
        Rejected,
        Completed
    }
} 