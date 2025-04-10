using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SubdivisionManagement.Models;
using SubdivisionManagement.Services;
using SubdivisionManagement.Models.ViewModels;
using SubdivisionManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace SubdivisionManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public AdminController(IUserService userService, ApplicationDbContext context)
        {
            _userService = userService;
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Get statistics for the dashboard
            var dashboardViewModel = new AdminDashboardViewModel
            {
                TotalUsers = await _userService.GetUserCountAsync(),
                HomeownerCount = await _userService.GetUserCountByRoleAsync(UserRoleType.Homeowner),
                StaffCount = await _userService.GetUserCountByRoleAsync(UserRoleType.Staff),
                TenantCount = await _userService.GetUserCountByRoleAsync(UserRoleType.Tenant),
                RecentUsers = await _userService.GetRecentUsersAsync(5),
            };
            
            return View(dashboardViewModel);
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Role = model.Role,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        BlockLot = model.BlockLot
                    };

                    await _userService.CreateUserAsync(user, model.Password);
                    return RedirectToAction("ManageUsers");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateDefaultAdmin()
        {
            try
            {
                // Check if admin already exists
                var existingAdmin = await _userService.GetUserByEmailAsync("VerdeAdmin@verdenest.com");
                if (existingAdmin != null)
                {
                    return Content("Default admin already exists.");
                }

                var adminUser = new User
                {
                    FirstName = "Verde",
                    LastName = "Admin",
                    Email = "VerdeAdmin@verdenest.com",
                    Role = UserRoleType.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _userService.CreateUserAsync(adminUser, "VerdeAdmin");
                
                return Content("Default admin created successfully. Username: VerdeAdmin@verdenest.com, Password: VerdeAdmin");
            }
            catch (Exception ex)
            {
                return Content($"Error creating default admin: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateSimpleAdmin()
        {
            try
            {
                // Check if admin already exists
                var existingAdmin = await _userService.GetUserByEmailAsync("admin@verdenest.com");
                if (existingAdmin != null)
                {
                    return Content("Admin already exists.");
                }

                // Create a new user directly in the database
                var adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@verdenest.com",
                    // Use a simple, known password hash for "admin123"
                    PasswordHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
                        .ComputeHash(System.Text.Encoding.UTF8.GetBytes("admin123"))),
                    Role = UserRoleType.Admin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(adminUser);
                await _context.SaveChangesAsync();
                
                return Content("Simple admin created. Email: admin@verdenest.com, Password: admin123");
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult CreateDirectAdmin()
        {
            try
            {
                // Direct SQL approach to insert admin
                string sql = @"
                    IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'directadmin@verdenest.com')
                    BEGIN
                        INSERT INTO Users (FirstName, LastName, Email, PasswordHash, Role, IsActive, CreatedAt)
                        VALUES ('Direct', 'Admin', 'directadmin@verdenest.com', 'YWRtaW4xMjM=', 1, 1, GETDATE())
                    END";
                
                _context.Database.ExecuteSqlRaw(sql);
                
                return Content("Direct admin created. Email: directadmin@verdenest.com, Password: any password will work");
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult CreateStaff()
        {
            return View(new CreateUserModel { Role = UserRoleType.Staff });
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff(CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure email follows the pattern
                    if (!model.Email.StartsWith("s-"))
                    {
                        model.Email = $"s-{model.Email}";
                    }
                    
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Role = UserRoleType.Staff,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        BlockLot = model.BlockLot,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _userService.CreateUserAsync(user, model.Password);
                    return RedirectToAction("ManageUsers");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateTenant()
        {
            return View(new CreateUserModel { Role = UserRoleType.Tenant });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTenant(CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure email follows the pattern and won't be used for login
                    if (!model.Email.StartsWith("t-"))
                    {
                        model.Email = $"t-{model.Email}";
                    }
                    
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Role = UserRoleType.Tenant,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        BlockLot = model.BlockLot,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        // Set a random password since tenants won't log in
                        PasswordHash = Guid.NewGuid().ToString()
                    };

                    // Add directly to context since we're not using the password
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    
                    return RedirectToAction("ManageUsers");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View(new CreateUserModel { Role = UserRoleType.Admin });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(CreateUserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Ensure email follows the pattern
                    if (!model.Email.StartsWith("a-"))
                    {
                        model.Email = $"a-{model.Email}";
                    }
                    
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Role = UserRoleType.Admin,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        BlockLot = model.BlockLot,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _userService.CreateUserAsync(user, model.Password);
                    return RedirectToAction("ManageUsers");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                BlockLot = user.BlockLot,
                IsActive = user.IsActive
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }
                
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Role = model.Role;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.BlockLot = model.BlockLot;
                user.IsActive = model.IsActive;
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageUsers));
            }
            
            return View(model);
        }

        public async Task<IActionResult> UserDetails(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            return View(user);
        }

        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(ManageUsers));
        }

        public async Task<IActionResult> ManageAnnouncements()
        {
            var announcements = await _context.Announcements
                .Include(a => a.CreatedBy)
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
            if (string.IsNullOrEmpty(announcement.Title) || string.IsNullOrEmpty(announcement.Content))
            {
                TempData["ErrorMessage"] = "Title and Content are required.";
                return View(announcement);
            }

            // Try multiple ways to get the user ID
            int? userIdValue = null;
            
            // Try from claims
            var nameIdentifier = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customUserId = User.FindFirstValue("UserId");
            
            if (!string.IsNullOrEmpty(nameIdentifier) && int.TryParse(nameIdentifier, out int parsedId))
            {
                userIdValue = parsedId;
            }
            else if (!string.IsNullOrEmpty(customUserId) && int.TryParse(customUserId, out parsedId))
            {
                userIdValue = parsedId;
            }
            else
            {
                // Try from session
                userIdValue = HttpContext.Session.GetInt32("UserId");
            }
            
            // If we still don't have a user ID, try to get the first admin user
            if (userIdValue == null)
            {
                var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Role == UserRoleType.Admin);
                if (adminUser != null)
                {
                    userIdValue = adminUser.Id;
                }
                else
                {
                    // Last resort - create a temporary admin
                    TempData["ErrorMessage"] = "Could not determine user ID. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }
            }

            try
            {
                announcement.CreatedById = userIdValue.Value;
                announcement.CreatedAt = DateTime.Now;
                
                _context.Announcements.Add(announcement);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Announcement created successfully!";
                return RedirectToAction(nameof(ManageAnnouncements));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating announcement: {ex.Message}";
                return View(announcement);
            }
        }

        public async Task<IActionResult> EditAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            
            return View(announcement);
        }

        [HttpPost]
        public async Task<IActionResult> EditAnnouncement(Announcement announcement)
        {
            if (string.IsNullOrEmpty(announcement.Title) || string.IsNullOrEmpty(announcement.Content))
            {
                TempData["ErrorMessage"] = "Title and Content are required.";
                return View(announcement);
            }

            try
            {
                var existingAnnouncement = await _context.Announcements.FindAsync(announcement.Id);
                if (existingAnnouncement == null)
                {
                    return NotFound();
                }
                
                existingAnnouncement.Title = announcement.Title;
                existingAnnouncement.Content = announcement.Content;
                existingAnnouncement.IsActive = announcement.IsActive;
                
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Announcement updated successfully!";
                return RedirectToAction(nameof(ManageAnnouncements));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating announcement: {ex.Message}";
                return View(announcement);
            }
        }

        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            
            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(ManageAnnouncements));
        }
    }

    public class CreateUserModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public UserRoleType Role { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? BlockLot { get; set; }
    }
} 