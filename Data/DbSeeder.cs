using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SubdivisionManagement.Models;
using SubdivisionManagement.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SubdivisionManagement.Data
{
    public class DbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public DbSeeder(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task SeedAsync()
        {
            // Create default admin if it doesn't exist
            if (!await _context.Users.AnyAsync(u => u.Email == "VerdeAdmin@verdenest.com"))
            {
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
                
                await _context.SaveChangesAsync();
                
                Console.WriteLine("Default admin user created.");
            }
        }
    }
} 