
using Microsoft.AspNetCore.Http;
using RaceDay.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers and views.
builder.Services.AddControllersWithViews();

// Allow services and controllers to access the current HTTP context.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<RaceDayApiService>();

// Configure communication with the existing RaceDay Part 2 API.
builder.Services.AddHttpClient("RaceDayApi", client =>
{
    string baseUrl = builder.Configuration["RaceDayApi:BaseUrl"]
        ?? throw new InvalidOperationException(
            "RaceDay API BaseUrl is missing from appsettings.json.");

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // API session cookies will be managed separately for each MVC user.
    UseCookies = false
});

// Enable server-side session storage.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    options.Cookie.Name = ".RaceDay.Web.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable session management before MVC controllers execute.
app.UseSession();

app.UseAuthorization();

// Configure the default MVC route.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
