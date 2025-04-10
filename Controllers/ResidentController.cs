using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubdivisionManagement.Data;
using SubdivisionManagement.Models;
using SubdivisionManagement.Models.ViewModels;
using SubdivisionManagement.Services;
using System.Threading.Tasks;

namespace SubdivisionManagement.Controllers
{
    public class ResidentController : Controller
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public ResidentController(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get recent announcements
            var recentAnnouncements = await _context.Announcements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Get total announcements count
            var totalAnnouncements = await _context.Announcements
                .Where(a => a.IsActive)
                .CountAsync();

            // Create the dashboard view model
            var viewModel = new ResidentDashboardViewModel
            {
                User = user,
                RecentAnnouncements = recentAnnouncements,
                TotalAnnouncements = totalAnnouncements,
                UserRole = user.Role.ToString(),
                Address = user.Address ?? "Not provided",
                BlockLot = user.BlockLot ?? "Not provided"
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Announcements()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var announcements = await _context.Announcements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(announcements);
        }

        public async Task<IActionResult> MyRequests()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var requests = await _context.Requests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            
            return View(requests);
        }

        public IActionResult CreateRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest(Request request)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                request.UserId = userId.Value;
                request.Status = RequestStatus.Pending;
                request.CreatedAt = DateTime.Now;
                
                _context.Requests.Add(request);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(MyRequests));
            }
            
            return View(request);
        }

        public async Task<IActionResult> RequestDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var request = await _context.Requests
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
                
            if (request == null)
            {
                return NotFound();
            }
            
            return View(request);
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(user);
        }

        public IActionResult CommunityEvents()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // For now, we'll just return a simple view
            return View();
        }

        public IActionResult PayDues()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // For now, we'll just return a simple view
            return View();
        }

        public IActionResult CommunityDirectory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            // For now, we'll just return a simple view
            return View();
        }
    }
} 