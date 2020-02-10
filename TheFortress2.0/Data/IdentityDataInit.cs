using System;
using Microsoft.AspNetCore.Identity;

namespace TheFortress.Data
{
    public class IdentityDataInit
    {
        // calls SeedUsers and SeedRoles
        public static void SeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);

        }
        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            // seed Admin account
            if (userManager.FindByNameAsync ("admin").Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "admin";
                user.Email = "mm751@humboldt.edu";

                IdentityResult result = userManager.CreateAsync
                (user, "P@ssw0rd!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }


            if (userManager.FindByNameAsync ("user1").Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "user1";
                user.Email = "user1@localhost";

                IdentityResult result = userManager.CreateAsync
                (user, "User1p@ssword").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "User").Wait();
                }
            }

            if (userManager.FindByNameAsync("user2").Result == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "user2";
                user.Email = "user2@localhost";

                IdentityResult result = userManager.CreateAsync
                (user, "User2p@assword").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Artist").Wait();
                }
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync ("Artist").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Artist";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync ("User").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "User";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync ("Trusted").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Trusted";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync ("Administrator").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Administrator";
                IdentityResult roleResult = roleManager.
                CreateAsync(role).Result;
            }
        }
    }
}
