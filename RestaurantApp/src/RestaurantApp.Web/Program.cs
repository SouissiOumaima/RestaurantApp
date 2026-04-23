using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantApp.Core.Interfaces;
using RestaurantApp.Infrastructure.Data;
using RestaurantApp.Infrastructure.Identity;
using RestaurantApp.Infrastructure.Repositories;
using RestaurantApp.Infrastructure.Services;
using RestaurantApp.Web;
var builder = WebApplication.CreateBuilder(args);

// ── Base de données ──────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ── ASP.NET Core Identity ────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ── Cookie ───────────────────────────────────────────────────
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

// ── Repository + UnitOfWork ──────────────────────────────────
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<RestaurantService>();
builder.Services.AddScoped<ReservationService>();
builder.Services.AddScoped<StatistiquesService>();
// ── MVC ──────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── Seed Rôles + Admin ───────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
                          .GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider
                          .GetRequiredService<UserManager<ApplicationUser>>();
    var context = scope.ServiceProvider
                          .GetRequiredService<AppDbContext>();

    await SeedData.InitializeAsync(roleManager, userManager, context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();