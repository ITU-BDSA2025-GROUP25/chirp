using Chirp.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;

var builder = WebApplication.CreateBuilder(args);
    //var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    //builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddRazorPages();
builder.Services.AddDbContext<ChirpDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")!));
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ChirpDbContext>();

//builder.Services.AddScoped<CheepService>();

// Load database connection 
/*var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDbContext>(options => options.UseSqlite(connectionString));*/


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
    context.Database.Migrate();  // Apply migrations automatically
    DbInitializer.SeedDatabase(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

public partial class Program { }