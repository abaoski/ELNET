using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubdivisionManagement.Data;
using SubdivisionManagement.Models;
using SubdivisionManagement.Models.ViewModels;
using System.Threading.Tasks;

namespace SubdivisionManagement.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Get current staff user from session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var staff = await _context.Users.FindAsync(userId);
            if (staff == null || staff.Role != UserRoleType.Staff)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get data for staff dashboard
            var dashboardViewModel = new StaffDashboardViewModel
            {
                TotalHomeowners = await _context.Users.CountAsync(u => u.Role == UserRoleType.Homeowner),
                TotalTenants = await _context.Users.CountAsync(u => u.Role == UserRoleType.Tenant),
                TotalAnnouncements = await _context.Announcements.CountAsync(),
                RecentAnnouncements = await _context.Announcements
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                PendingRequests = await _context.Requests
                    .Where(r => r.Status == RequestStatus.Pending)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboardViewModel);
        }

        public async Task<IActionResult> ManageAnnouncements()
        {
            var announcements = await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            
            return View(announcements);
        }

        public IActionResult CreateAnnouncement()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement(Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                announcement.CreatedById = userId.Value;
                announcement.CreatedAt = DateTime.Now;
                
                _context.Announcements.Add(announcement);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(ManageAnnouncements));
            }
            
            return View(announcement);
        }

        public async Task<IActionResult> ManageRequests()
        {
            var requests = await _context.Requests
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            
            return View(requests);
        }

        public async Task<IActionResult> RequestDetails(int id)
        {
            var request = await _context.Requests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
                
            if (request == null)
            {
                return NotFound();
            }
            
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRequestStatus(int id, RequestStatus status)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            
            request.Status = status;
            request.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(ManageRequests));
        }
    }
} 