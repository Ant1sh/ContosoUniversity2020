using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity2020.Data
{
    //Part 11: Identity frimework
    //1. Seeding roles and admin user
    public class IdentityDbInitializer
    {
        public  static async Task Initialize(IServiceProvider serviceProvider,string adminUserPW)
        {
            //1.initialize coustom roles
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //2. These will be the roles
            string[] roleNames = { "Admin", "Student", "Instructor" };
            //3. Prepare result variable
            IdentityResult roleResult;
            //4. Loop the roleNames array - check if role already exists, and create new role if nessesary
            foreach(var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //role does not exists - create it
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));

                }
                //check if statements
            }
            //end create roles
            //create admin user
            //1. initialize custom user(s) - in this case, just the admin user
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            //2. check if the admin is already exists
            IdentityUser adminUser = await UserManager.FindByEmailAsync("admin@contoso.com");
            if(adminUser == null)
            {
                //admin user does not already exists - creatr it 
                adminUser = new IdentityUser()
                {
                    UserName = "admin@contoso.com",
                    Email = "admin@contoso.com"
                };
                // now actualy create it
                await UserManager.CreateAsync(adminUser, adminUserPW);
                //the adminUserPW is for storing the password - it will be passed in via
                //Startup (using Dependency Injection). The actual password will be kept in
                //a file called Secrets.json

                //Assign adminUser to AdminRole
                await UserManager.AddToRoleAsync(adminUser, "Admin");
                //manually confirm the admin user`s email
                var code = await UserManager.GenerateEmailConfirmationTokenAsync(adminUser);
                var result = await UserManager.ConfirmEmailAsync(adminUser, code);
                //No account for admin user
                await UserManager.SetLockoutEnabledAsync(adminUser, false);

            }

        }





    }
}
