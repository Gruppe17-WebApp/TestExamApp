using TestExamApp.Services;
using Microsoft.EntityFrameworkCore;
using TestExamApp.Data;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// API service
builder.Services.AddHttpClient<BookApiService>();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// MVC
builder.Services.AddControllersWithViews();

// Identity UI
builder.Services.AddRazorPages();

var app = builder.Build();

// Norsk kultur (SKAL være her – før requests starter)
var culture = new CultureInfo("nb-NO");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// riktig rekkefølge
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Identity routes
app.MapRazorPages();

// Seed + migrate
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();
    await SeedData.InitializeAsync(context);
}

app.Run();