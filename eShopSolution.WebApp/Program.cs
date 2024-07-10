using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using eShopSolution.ApiIntegration;
using FluentValidation.AspNetCore;
using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using eShopSolution.ViewModels.System.Users;
using eShopSolution.WebApp.LocalizationResources;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;
var cultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("vi"),
};

// Add services
builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LoginRequestValidator>())
    .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>(ops =>
    {
        ops.ResourcesPath = "LocalizationResources";
        ops.RequestLocalizationOptions = o =>
        {
            o.SupportedCultures = cultures;
            o.SupportedUICultures = cultures;
            o.DefaultRequestCulture = new RequestCulture("vi");
        };
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/User/Forbidden/";
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ISlideApiClient, SlideApiClient>();
builder.Services.AddTransient<IProductApiClient, ProductApiClient>();
builder.Services.AddTransient<ICategoryApiClient, CategoryApiClient>();
builder.Services.AddTransient<IUserApiClient, UserApiClient>();

var app = builder.Build();

// Configure the app
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.UseRequestLocalization();

// Configure endpoint routing
app.MapControllerRoute(
    name: "Product Category En",
    pattern: "{culture}/categories/{id}",
    defaults: new { controller = "Product", action = "Category" });

app.MapControllerRoute(
    name: "Product Category Vn",
    pattern: "{culture}/danh-muc/{id}",
    defaults: new { controller = "Product", action = "Category" });

app.MapControllerRoute(
    name: "Product Detail En",
    pattern: "{culture}/products/{id}",
    defaults: new { controller = "Product", action = "Detail" });

app.MapControllerRoute(
    name: "Product Detail Vn",
    pattern: "{culture}/san-pham/{id}",
    defaults: new { controller = "Product", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{culture=vi}/{controller=Home}/{action=Index}/{id?}");

app.Run();
