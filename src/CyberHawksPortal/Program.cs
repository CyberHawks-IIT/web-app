using CyberHawksPortal.Data;
using CyberHawksPortal.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Allow running as a Windows Service (no-op when launched as a console app in dev).
builder.Host.UseWindowsService();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Portal")));

var app = builder.Build();

// Create & seed the database on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbSeeder.Seed(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// NOTE: served over plain HTTP in the range (cleartext credential transmission is intentional).
app.UseStatusCodePagesWithReExecute("/oops", "?code={0}");
// Serve static files, including leftover backups like Web.config.bak and db_backup.sql
// (unknown extensions served as text/plain) — intentional sensitive-file exposure (#28).
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/plain"
});

app.UseRouting();

app.UseMiddleware<CurrentUserMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
