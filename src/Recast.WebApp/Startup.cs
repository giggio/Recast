using Microsoft.AspNetCore.Routing.Constraints;
using Recast.WebApp.Models;

namespace Recast.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<Feeds>();
            services.AddScoped<Posts>();
            services.MapAll();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AzureStorage.CreateAllTablesAsync(Configuration).Wait();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "XMLFeed",
                    pattern: "{userName}/{feedName}",
                    defaults: new { controller = "Feeds", action = "GetFeed" },
                    constraints: new { userName = @"^(?!feeds$|feed$|home$).*" }
                );
                endpoints.MapControllerRoute(
                    name: "ViewFeed",
                    pattern: "Feeds/{userName:regex(^(?!create$|Create$|new$|New$).*)}/{feedName}",
                    defaults: new { controller = "Feeds", action = "ViewFeed" },
                    constraints: new { httpMethod = new HttpMethodRouteConstraint("GET") }
                );
                endpoints.MapControllerRoute(
                    name: "Default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
