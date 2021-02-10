using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spice.Models;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async void Initialize()
        {
            //instead of await and async method we can add ACTION_ASYNC.GetAwaiter().GetResult()

            //If there any panding Migrations then apply them
            var x = await _db.Database.CanConnectAsync();
            try
            {
                var x1 = _db.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count();
                if (_db.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0) 
                {
                    await _db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (_db.Roles.AnyAsync(r => r.Name.Equals(SD.ManagerUser)).GetAwaiter().GetResult()) return;

            foreach (var r in SD.UserArray)
            {
                if (!_roleManager.RoleExistsAsync(r).GetAwaiter().GetResult())
                {
                    //if role doesn't exist create it
                    _roleManager.CreateAsync(new IdentityRole(r)).GetAwaiter().GetResult();
                }
            }

            _userManager.CreateAsync(new ApplicationUser 
            {
                UserName = SD.AdminAccountInfo.UserName,
                Email = SD.AdminAccountInfo.Email,
                EmailConfirmed = SD.AdminAccountInfo.EmailConfirmed,
                PhoneNumber = SD.AdminAccountInfo.Phone,
                FirstName = SD.AdminAccountInfo.Name.Split(" ")[0],
                LastName = SD.AdminAccountInfo.Name.Split(" ")[1],
            },SD.AdminAccountInfo.Password).GetAwaiter().GetResult();

            IdentityUser user = await _db.Users.FirstOrDefaultAsync(u => u.Email.Equals(SD.AdminAccountInfo.Email));

            await _userManager.AddToRoleAsync(user,SD.ManagerUser);


        }
    }
}
