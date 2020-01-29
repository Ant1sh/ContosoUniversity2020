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
using ContosoUniversity2020.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ContosoUniversity2020
{
    public class Startup
    {
        //part11:Identity framework
        //create privat member for reading the sercet key
        private string _adminUserPW = null;
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
            //anton: part3 : register the schoolcontext (databasecontext) using Dependency Injection (DI)
            services.AddDbContext<SchoolContext>(options => // change to schoolcontext
               options.UseSqlServer(
                   Configuration.GetConnectionString("DefaultConnection")));
            //end part3

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            //services.AddControllersWithViews();
            //services.AddRazorPages();

            //part 11: identity frimework
            //add roles and default ui
            services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
              .AddDefaultUI()//for routing identity pages (razor pages, not mvc)
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();
            

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, //part 5 changes
                   /*SchoolContext context)*/IServiceProvider serviceProvider)//changes end
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

            // part 10 : Custom reeor pages ( for 404 and 500 status codes )
            //note: Must calll the UseStatusCodePages before request handling middlewares
            //like Static Files and MVC middlewares
            app.UseStatusCodePagesWithReExecute("/Error/{0}");//send to error controller passing status code argument
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //part 5 changes
            //DbInitializer.Initialize(context);
            //changes end
            //Part 11: Identity Framework (initialize identity users and roles)
            //first we need to get our admin password
            _adminUserPW = Configuration["adminUserPW"];
            //now we can call our IdantityDbInitializer
            IdentityDbInitializer.Initialize(serviceProvider, _adminUserPW).Wait();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
