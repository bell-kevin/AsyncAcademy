using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AsyncAcademy.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AsyncAcademy.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AsyncAcademyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TITAN-CONNECTION-STRING") ?? throw new InvalidOperationException("Connection string 'TITAN-CONNECTION-STRING' not found.")));

// From Bash
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(300);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//

// Configure Stripe using secret key
StripeConfiguration.ApiKey = "sk_test_51Q2OkdLffhLXA6EXPrfKh0VcjFq9OydxjlpNAE9tAFVygmAWLSrmKYjjwo2HliwAIicTtxat7XS5OLqAEMw1Ooce00JpCHPLkh";  // Secret key



builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // The Last Part

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Upon service creation, check logic for seeding an empty database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var SeedData = new SeedData(scope.ServiceProvider.GetRequiredService<AsyncAcademyContext>());
    SeedData.Initialize(services);
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

app.UseAuthorization();

app.UseSession(); // From Bash

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();