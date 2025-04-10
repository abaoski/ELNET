using Microsoft.EntityFrameworkCore;
using SubdivisionManagement.Data;
using SubdivisionManagement.Services;

namespace VerdeNest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add DbContext using SQLite
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add User Service
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddControllersWithViews();

            // Add session services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Test database connection
            try
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    context.Database.EnsureCreated(); // Use EnsureCreated instead of migrations
                    Console.WriteLine("Successfully connected to the database!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to the database: {ex.Message}");
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Add this after your service registrations but before app.Run()
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    var userService = services.GetRequiredService<IUserService>();
                    var seeder = new DbSeeder(dbContext, userService);
                    seeder.SeedAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Add session middleware
            app.UseSession();

            // Comment out migration code that was causing errors
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var dbContext = services.GetRequiredService<ApplicationDbContext>();
            //    dbContext.Database.Migrate();
            //}

            // Add this right after building the app but before app.Run()
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    
                    // This will create the database if it doesn't exist
                    dbContext.Database.EnsureCreated();
                    
                    // Check if Announcements table exists, if not create it
                    if (!dbContext.Announcements.Any())
                    {
                        // Create a test announcement
                        var adminUser = dbContext.Users.FirstOrDefault(u => u.Role == UserRoleType.Admin);
                        if (adminUser != null)
                        {
                            dbContext.Announcements.Add(new Announcement
                            {
                                Title = "Welcome to VerdeNest",
                                Content = "This is a test announcement to ensure the announcements feature is working correctly.",
                                CreatedById = adminUser.Id,
                                CreatedAt = DateTime.Now,
                                IsActive = true
                            });
                            dbContext.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while ensuring the database was created.");
                }
            }

            app.Run();
        }
    }
}
