using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Shop.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Serilog to log errors to a file
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error() // Log only errors and above
    .WriteTo.File(@"C:\Logs\log.txt", rollingInterval: RollingInterval.Day) // Log to a file, roll everyday
    .CreateLogger();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShopPortal")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseSerilogRequestLogging(); // Log requests

// Log unhandled exceptions
/*app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500; // Set HTTP status code to 500
        Log.Error(context.Features.Get<IExceptionHandlerFeature>().Error, "Unhandled exception occurred"); // Log unhandled exception
        await context.Response.WriteAsync("An unexpected error occurred. Please try again later."); // Send a generic error message to the client
    });
});*/

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
