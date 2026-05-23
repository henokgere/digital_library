using digital_library.Data;
using digital_library.Models;
using Microsoft.EntityFrameworkCore;

LoadDotEnv(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<digital_libraryContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("digital_libraryContext")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// HttpClient is used to call Google's OAuth token/userinfo endpoints.
builder.Services.AddHttpClient();

// Session is used to keep the signed-in user across requests.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Loads KEY=VALUE pairs from a .env file into process env vars for local dev.
// Real env vars (set by the OS or by Render) always win.
static void LoadDotEnv(string path)
{
    if (!File.Exists(path)) return;

    foreach (var raw in File.ReadAllLines(path))
    {
        var line = raw.Trim();
        if (line.Length == 0 || line.StartsWith('#')) continue;

        var eq = line.IndexOf('=');
        if (eq <= 0) continue;

        var key = line[..eq].Trim();
        var value = line[(eq + 1)..].Trim().Trim('"', '\'');

        if (Environment.GetEnvironmentVariable(key) is null)
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
