using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook
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

            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<EmailOptions>(Configuration);
            services.AddRazorPages();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });
            services.AddAuthentication().AddFacebook(options => {
                options.AppId = "829391274385672";
                options.AppSecret = "6c75ddad1472deb8d570e820172a898d";
            });
            services.AddAuthentication().AddGoogle(options => 
            {
                options.ClientId = "811538936123-f3gbns33qau63mlcmi9u225196bg63rt.apps.googleusercontent.com";
                options.ClientSecret = "cYiVyRgZVuaObZ2mlyF6dzLA";
            });

            services.AddSession(options => 
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;

            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
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
