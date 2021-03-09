using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using spice.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using spice.Utility;
using Stripe;
using spice.Service;

namespace spice
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>()
               .AddDefaultTokenProviders()
               .AddDefaultUI()
               .AddEntityFrameworkStores<ApplicationDbContext>();

            // Seed data
            services.AddScoped<IDbInitializer, DbInitializer>();
            // For Payments
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
           
            // For Email Service
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<EmailOptions>(Configuration);


            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation();

            // Sign in by FB and Google
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = "393432528703020";
                facebookOptions.AppSecret = "1744255b1c4d9b54ccb55593fc710fa8";
            });

            //services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = "943287545908-23cg1r49vf777q6lj9ndvl72cfro6vac.apps.googleusercontent.com";
            //    googleOptions.ClientSecret = "9ppq7O-KTSbX4GNl8hWJscnG";
            //});

            //Add Session
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });

            // Add if you face any error in Authorization
            //services.ConfigureApplicationCookie(options =>

            //{

            //    options.LoginPath = $"/Identity/Account/Login";

            //    options.LogoutPath = $"/Identity/Account/Logout";

            //    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";

            //});
            //services.AddSingleton<IEmailSender, IEmailSender>();
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
            //Add Strip for payment methods
            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);

            dbInitializer.Initialize();
            // Add Session
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

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
