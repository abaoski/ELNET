using System.Collections.Generic;
using SubdivisionManagement.Models;

namespace SubdivisionManagement.Models.ViewModels
{
    public class StaffDashboardViewModel
    {
        public int TotalHomeowners { get; set; }
        public int TotalTenants { get; set; }
        public int TotalAnnouncements { get; set; }
        public List<Announcement> RecentAnnouncements { get; set; } = new List<Announcement>();
        public List<Request> PendingRequests { get; set; } = new List<Request>();
    }
} 