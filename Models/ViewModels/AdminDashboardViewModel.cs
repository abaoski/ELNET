using System.Collections.Generic;

namespace SubdivisionManagement.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int HomeownerCount { get; set; }
        public int StaffCount { get; set; }
        public int TenantCount { get; set; }
        public List<User> RecentUsers { get; set; } = new List<User>();
        // Add other statistics properties as needed
    }
} 