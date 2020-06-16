using EstateApp.Data.DatabaseContext.AuthenticationDbContext;
using EstateApp.Data.DatabaseContext.ApplicationDbContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EstateApp.Data;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace EstateApp.Web
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
            //Added DbContext service
            services.AddDbContextPool<AuthenticationDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("AuthenticationConnection"),
            
            sqlServerOptions => 
            {
                    sqlServerOptions.MigrationsAssembly("estateapp.data");
            }
            ));
            services.AddDbContextPool<ApplicationDbContext>(options => 
            options.UseSqlServer(
                Configuration.GetConnectionString("ApplicationConnection"),
            
            //Migration is created in EstateApp.Data Folder
            sqlServerOptions =>
            {
                sqlServerOptions.MigrationsAssembly("estateapp.data");
            }));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthenticationDbContext>()
                .AddDefaultTokenProviders();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            
            MigrationDatabaseContext(app);
            CreateDefaultRolesandUserAsync(app).GetAwaiter().GetResult();
        }
        
        //This method configures the Migration Database Context Class
        public void MigrationDatabaseContext(IApplicationBuilder app)
        {
          var authenticationDbContext =  app.ApplicationServices.GetRequiredService<AuthenticationDbContext>();
          authenticationDbContext.Database.MigrateAsync(); 

          var applicationDbContext = app.ApplicationServices.GetRequiredService<ApplicationDbContext>();
          applicationDbContext.Database.MigrateAsync();
        }
    
        //Method creates Roles and users
        public async Task CreateDefaultRolesandUserAsync(IApplicationBuilder app)
        {
            string[] roles = new string[]{"SystemAdminstrator","Agent","User"};
            var userEmail = "admin@estateapp.com";
            var userPassword = "Super@2020";
            
            //gets roleManager Service
            var roleManager = app.ApplicationServices.GetRequiredService<RoleManager<string>>();
            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(role);
                }
            }
            //gets userManager service
            var userManager = app.ApplicationServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByEmailAsync(userEmail);
            
            if(user is null)
            {
                user = new ApplicationUser
                {
                    Email = userEmail,
                    UserName = userEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "+2347065712398",
                    PhoneNumberConfirmed = true
                };
                await userManager.CreateAsync(user, userPassword);
                await userManager.AddToRolesAsync(user, roles);
            }

            
        } 
    
    }
}


