using Microsoft.AspNetCore.Routing.Constraints;
using Recast.WebApp.Models;

var builder = WebApplication.CreateBuilder(args);                                                                                                                                                                                                                                                                           // Add services to the container.
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<Feeds>();
builder.Services.AddScoped<Posts>();
builder.Services.MapAll();

var app = builder.Build();
var config = app.Services.GetRequiredService<IConfiguration>();
await AzureStorage.CreateAllTablesAsync(config);

if (app.Environment.IsDevelopment())
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

app.Run();

