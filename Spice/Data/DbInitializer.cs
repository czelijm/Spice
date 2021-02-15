﻿using Microsoft.AspNetCore.Identity;
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

        public void Initialize() // never use async void XD
        {
            //instead of await and async method we can add ACTION_ASYNC.GetAwaiter().GetResult()



            //var x = _db.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count();
            //If there any panding Migrations then apply them
            try
            {
                //var canDbConnect = await _db.Database.CanConnectAsync();
                //if db doesn't exist, create it
                //var isDbNewlyCreated = await _db.Database.EnsureCreatedAsync();
                //var x1 = _db.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count();

                if (_db.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0) 
                {
                    _db.Database.Migrate();
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

            IdentityUser user = _db.Users.FirstOrDefaultAsync(u => u.Email.Equals(SD.AdminAccountInfo.Email)).GetAwaiter().GetResult();

            _userManager.AddToRoleAsync(user,SD.ManagerUser).GetAwaiter().GetResult();


        }
    }
}
