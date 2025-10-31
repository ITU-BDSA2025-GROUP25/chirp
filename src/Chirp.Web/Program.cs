using Chirp.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNet.Security.OAuth.GitHub;

var builder = WebApplication.CreateBuilder(args);


// Application services (Cheep, Author, etc.)
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

// Razor pages for the web UI
builder.Services.AddRazorPages();

// Database setup (SQLite)
builder.Services.AddDbContext<ChirpDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")!));

// ASP.NET Core Identity for local login
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ChirpDbContext>();


// Github OAUTH2 configtuation
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "GitHub";
})
.AddCookie()
.AddGitHub(options =>
{
    // Securely load secrets from user-secrets for localhost or configuration from azure
    options.ClientId = builder.Configuration["authentication:github:clientId"];
    options.ClientSecret = builder.Configuration["authentication:github:clientSecret"];
    
    options.CallbackPath = "/signin-github";
});


//  BUILD THE APPLICATION
var app = builder.Build();

// Apply any pending database migrations and seed demo data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
    context.Database.Migrate();
    DbInitializer.SeedDatabase(context);
}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


// Razor pages
app.MapRazorPages();

// routes for GitHub login/logout
app.MapGet("/login", async context =>
{
    var props = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
    {
        RedirectUri = "/"
    };
    await context.ChallengeAsync("GitHub", props);
});

app.MapPost("/logout", async context =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    context.Response.Redirect("/");
});

app.Run();

// Partial Program class for integration testing
public partial class Program { }
