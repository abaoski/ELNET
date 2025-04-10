using System.Collections.Generic;
using SubdivisionManagement.Models;

namespace SubdivisionManagement.Models.ViewModels
{
    public class ResidentDashboardViewModel
    {
        public User User { get; set; }
        public List<Announcement> RecentAnnouncements { get; set; } = new List<Announcement>();
        public int TotalAnnouncements { get; set; }
        public string UserRole { get; set; }
        public string Address { get; set; }
        public string BlockLot { get; set; }
    }
} 