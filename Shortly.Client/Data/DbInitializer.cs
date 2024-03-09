using Microsoft.AspNetCore.Identity;
using Shortly.Client.Helpers.Roles;
using Shortly.Data;
using Shortly.Data.Models;

namespace Shortly.Client.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDefaultUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();


                //Simple user related data
                var simpleUserRole = Role.User;
                var simpleUserEmail = "user@shrtly.com";

                if (!await roleManager.RoleExistsAsync(simpleUserRole))
                    await roleManager.CreateAsync(new IdentityRole() { Name = simpleUserRole });

                if(await userManager.FindByEmailAsync(simpleUserEmail) == null)
                {
                    var simpleUser = new AppUser()
                    {
                        FullName = "Simple User",
                        UserName = "simple-user",
                        Email = simpleUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(simpleUser, "Coding@1234?");

                    //Add user to the role
                    await userManager.AddToRoleAsync(simpleUser, simpleUserRole);
                }

                //Admin user related data
                var adminUserRole = Role.Admin;
                var adminUserEmail = "admin@shrtly.com";

                if (!await roleManager.RoleExistsAsync(adminUserRole))
                    await roleManager.CreateAsync(new IdentityRole() { Name = adminUserRole });

                if (await userManager.FindByEmailAsync(adminUserEmail) == null)
                {
                    var adminUser = new AppUser()
                    {
                        FullName = "Admin User",
                        UserName = "admin-user",
                        Email = adminUserEmail,
                        EmailConfirmed = true
                    };
                    await userManager.CreateAsync(adminUser, "Coding@1234?");

                    //Add user to the role
                    await userManager.AddToRoleAsync(adminUser, adminUserRole);
                }

            }
        }
    }
}
