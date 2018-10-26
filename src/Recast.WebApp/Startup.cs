using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<Feeds>();
            services.AddScoped<Posts>();
            AutoMapping.MapAll();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            AzureStorage.CreateAllTables(Configuration).Wait();
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
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "XMLFeed",
                    template: "{userName}/{feedName}",
                    defaults: new { controller = "Feeds", action = "GetFeed" },
                    constraints: new { userName = @"^(?!feeds$|feed$|home$).*" }
                );
                routes.MapRoute(
                    name: "ViewFeed",
                    template: "Feeds/{userName:regex(^(?!create$|Create$|new$|New$).*)}/{feedName}",
                    defaults: new { controller = "Feeds", action = "ViewFeed" },
                    constraints: new { httpMethod = new HttpMethodRouteConstraint("GET") }
                );
                routes.MapRoute(
                    name: "Default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
