using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql.Logging;
using Spice.Data;
using Spice.Services;
using Spice.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ////connectionString form appsetting.json //OK!
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));


            var dbProvider = Environment.GetEnvironmentVariable("DB_PROVIDER") ?? SD.DBProvier.postgres;
            var dbServerName = Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost";
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
            var dataBase = Environment.GetEnvironmentVariable("DB_DATA_BASE") ?? "Spice";
            var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "<YourStrong!Passw0rd>";
            var dbItegratedSecurity = Environment.GetEnvironmentVariable("DB_INTEGRATED_SECURITY") ?? "False";
            var dbPersistSecurityInfo = Environment.GetEnvironmentVariable("DB_PERSIST_SECURITY") ?? "False";

            //ExtraSpaceNeeded??!!
            //Environment.GetEnvironmentVariable("NAME_OF_VARIABLE ");

            //initialize context depend on db provider
            //switch (dbProvider.ToLower()) 
            //{  
            //    case SD.DBProvier.sqlServer:
            //    {
            //        services.AddDbContext<ApplicationDbContext>(options =>
            //            options.UseNpgsql
            //            (
            //                $"Server={dbServerName},{dbPort};Database={dataBase};Integrated Security={dbItegratedSecurity};Persist Security Info={dbPersistSecurityInfo};" +
            //                $"User ID={dbUser};Password={dbPassword}"
            //            )
            //        );
            //    }
            //        break;

            //    case SD.DBProvier.postgres:
            //    {
            //        services.AddDbContext<ApplicationDbContext>(options =>
            //            options.UseNpgsql
            //            (
            //                $"Server={dbServerName},{dbPort};Database={dataBase};Integrated Security={dbItegratedSecurity};Persist Security Info={dbPersistSecurityInfo};" +
            //                $"User ID={dbUser};Password={dbPassword}"
            //            )
            //        );

            //    }
            //        break;

            //    default:
            //    {
            //        throw new Exception($"Database provider not recognized:{dbProvider}"); 
            //    }
            //        //break;
            //}


            //new case switch syntax
            services.AddDbContext<ApplicationDbContext>(options => _  = dbProvider.ToLower() switch
                {
                    SD.DBProvier.sqlServer => options.UseSqlServer
                    (
                        $"Server={dbServerName},{dbPort};Database={dataBase};Integrated Security={dbItegratedSecurity};Persist Security Info={dbPersistSecurityInfo};" +
                        $"User ID={dbUser};Password={dbPassword}"
                    ),

                    //the postres's connection string uses diffrent keywords than sqlServer's connection string 
                    SD.DBProvier.postgres => options.UseNpgsql
                    (
                        $"Host={dbServerName};Port={dbPort};Database={dataBase};" +
                        $"User ID={dbUser};Password={dbPassword}"
                    ),

                    _ => throw new Exception($"Database provider not recognized:{dbProvider}"),
                }
            );

            //NpgsqlLogManager.Provider = new ConsoleLoggingProvider();


            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //to add role to user we have to use AddIdentity
            //services.AddScoped<IEmailSender, EmailSender>();
            services.AddIdentity<IdentityUser,IdentityRole>()
                //.AddDefaultUI()
                .AddDefaultTokenProviders() // if someone forgot password
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //Add dbInitializer to scope, we'll run initialize() in Configure()
            services.AddScoped<IDbInitializer, DbInitializer>();

            //if classes have got this same name we dont need the section
            //if members of classes has got this same as the json's section members name we don't have to add additional settings
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            // fixing Register/Login error by Bhurgel
            //add singleton, emailsender only one instance, we don't have to initialize/configure it again
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<EmailOptions>(Configuration.GetSection("SendGrip"));


            //for logIn logOut redirection
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            services.AddAuthentication().AddFacebook(facebookOptions=> 
            {
                facebookOptions.AppId = Configuration.GetSection("Facebook")["AppId"];
                facebookOptions.AppSecret = Configuration.GetSection("Facebook")["AppSecret"];
            });

            services.AddControllersWithViews();
            services.AddSession(options=> {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30); //session will be abbadoned after 30min of lack of the users's action
                options.Cookie.HttpOnly = true;
            });
            services.AddRazorPages().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            StripeConfiguration.ApiKey = Configuration.GetSection("Stripe")["SecretKey"];
            
            SD.CompanyInformations.emailAdmin = Configuration.GetSection("SendGrip")["EmailCompany"];

            SD.AdminAccountInfo.UserName = Configuration.GetSection("AdminAccountInfo")["UserName"];
            SD.AdminAccountInfo.Name = Configuration.GetSection("AdminAccountInfo")["Name"];
            SD.AdminAccountInfo.Email = Configuration.GetSection("AdminAccountInfo")["Email"];
            SD.AdminAccountInfo.EmailConfirmed = bool.Parse(Configuration.GetSection("AdminAccountInfo")["EmailConfirmed"]);
            SD.AdminAccountInfo.Phone = Configuration.GetSection("AdminAccountInfo")["Phone"];
            SD.AdminAccountInfo.Password = Configuration.GetSection("AdminAccountInfo")["Password"];
            dbInitializer.Initialize();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
