using BugTracker.Shared.Constants;
using BugTracker.API.Entities;
using Microsoft.AspNetCore.Identity;

namespace BugTracker.API.Seeder;

public static class RoleSeeder
{
    public static async Task Seed(IServiceProvider serviceProvider)
    {
        try
        {
            using (var scope = serviceProvider.CreateScope())
            {
                string[] roles = { DefaultRoleConstant.User, DefaultRoleConstant.Developer};
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AuthRole>>();

                foreach (var item in roles)
                {
                    // Check if the role exists
                    if (!await roleManager.RoleExistsAsync(item))
                    {
                        // Create the role
                        var userRole = new AuthRole { Name = item };
                        await roleManager.CreateAsync(userRole);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred during seeding: " + ex.Message);
        }
    }
}
