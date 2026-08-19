using Microsoft.AspNetCore.Localization;
using LFPortal.Application.Interfaces;
using LFPortal.Web.Demo;

var builder = WebApplication.CreateBuilder(args);
var demo = builder.Configuration.GetSection(DemoModeOptions.SectionName).Get<DemoModeOptions>()
           ?? new DemoModeOptions();

if (!demo.Enabled || !demo.UseMockData || !demo.BypassAuthentication)
    throw new InvalidOperationException("This copied build is demo-only. DemoMode must remain fully enabled.");

builder.Services.Configure<DemoModeOptions>(builder.Configuration.GetSection(DemoModeOptions.SectionName));
builder.Services.AddControllersWithViews().AddViewLocalization();
builder.Services.AddLocalization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Dashboard.Demo.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<ILaserficheDashboardService, MockDashboardService>();

var app = builder.Build();
app.UseStaticFiles();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("ar").AddSupportedCultures("ar", "en").AddSupportedUICultures("ar", "en"));
app.UseRouting();
app.UseSession();

// The health response is intentionally static: it performs no dependency checks.
app.MapGet("/health", () => Results.Json(new
{
    app = "Healthy", dataSource = "Mock", api = "Disabled in Demo Mode", authentication = "Bypassed"
}));
app.MapControllerRoute("default", "{controller=Dashboard}/{action=Index}/{id?}");
app.Run();

public partial class Program { }
