using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services
builder.Services.AddControllersWithViews();

// Add session and memory cache services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);       // Session timeout
    options.Cookie.HttpOnly = true;                        // Secure cookies
    options.Cookie.IsEssential = true;                     // Required for GDPR
});

// Add EF Core with SQL Server and retry-on-failure (transient resiliency)
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ToDoContext"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    )
);

var app = builder.Build();

// Middleware pipeline configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();              // Enable session handling
app.UseAuthorization();        // Add authorization support

// Route configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
