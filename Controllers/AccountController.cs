using Microsoft.AspNetCore.Mvc;
using SubdivisionManagement.Models;
using SubdivisionManagement.Services;

namespace SubdivisionManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if user is already logged in (you'll need to implement session management)
            if (HttpContext.Session.GetString("UserEmail") != null)
            {
                // Redirect based on role stored in session
                string userRoleString = HttpContext.Session.GetString("UserRole");
                if (!string.IsNullOrEmpty(userRoleString) && 
                    Enum.TryParse<SubdivisionManagement.Models.UserRoleType>(userRoleString, out var userRole) && 
                    userRole == SubdivisionManagement.Models.UserRoleType.Admin)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.AuthenticateAsync(model.Email, model.Password);
                if (user != null)
                {
                    // Store user ID in session
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                    HttpContext.Session.SetString("UserRole", user.Role.ToString());

                    // Redirect based on user role
                    switch (user.Role)
                    {
                        case UserRoleType.Admin:
                            return RedirectToAction("Dashboard", "Admin");
                        case UserRoleType.Staff:
                            return RedirectToAction("Dashboard", "Staff");
                        case UserRoleType.Homeowner:
                        case UserRoleType.Tenant:
                            return RedirectToAction("Dashboard", "Resident");
                        default:
                            return RedirectToAction("Index", "Home");
                    }
                }
                
                ModelState.AddModelError("", "Invalid email or password");
            }
            
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
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
                        Role = SubdivisionManagement.Models.UserRoleType.Homeowner
                    };

                    await _userService.CreateUserAsync(user, model.Password);
                    return RedirectToAction("Login");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CheckUserHash(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                return Content($"User found: {user.Email}, Hash: {user.PasswordHash}");
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
} 