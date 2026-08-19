using LFPortal.Domain.Version;
using LFPortal.Web.Demo;
using Serilog;

// This copied repository is intentionally a permanent standalone demo.
// It never registers or executes Laserfiche/LFDS/OAuth/SSO production services.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting {Display} in standalone demo mode", LFPortalVersion.Display);

    var builder = WebApplication.CreateBuilder(args);

    // Host-friendly logging only. No machine-wide or ProgramData file writes.
    builder.Host.UseSerilog((context, services, loggerConfig) =>
    {
        loggerConfig
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", LFPortalVersion.Display);
    });

    builder.Services.AddLocalization();

    // Keep the existing Razor UI and controllers exactly as they are.
    // Settings mutations are intercepted by the demo-safe action filter.
    builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<DemoSettingsActionFilter>();
        })
        .AddViewLocalization();

    // Register hardcoded/mock services only.
    builder.Services.AddDashboardDemoMode();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.Name = ".Dashboard.Demo.Session";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.IdleTimeout = TimeSpan.FromHours(8);
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    });

    app.UseStaticFiles();

    var supportedCultures = new[] { "en", "ar" };
    app.UseRequestLocalization(new RequestLocalizationOptions()
        .SetDefaultCulture("en")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures));

    app.UseRouting();
    app.UseSession();

    // Blocks all legacy integration/auth/document routes before controller activation.
    app.UseMiddleware<DemoRouteSafetyMiddleware>();

    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = WriteHealthResponseAsync
    });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Dashboard}/{action=Index}/{id?}");

    Log.Information(
        "{Display} started. Repository={Repository}; data source=hardcoded demo data",
        LFPortalVersion.Display,
        DemoDataStore.RepositoryId);

    await app.RunAsync();
}
catch (Exception ex) when (ex is not OperationCanceledException && ex.GetType().Name != "HostAbortedException")
{
    Log.Fatal(ex, "{Display} terminated unexpectedly", LFPortalVersion.Display);
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

return 0;

static async Task WriteHealthResponseAsync(
    HttpContext context,
    Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport report)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var payload = new
    {
        version = LFPortalVersion.Full,
        mode = "Standalone Demo",
        dataSource = "Hardcoded",
        repository = DemoDataStore.RepositoryId,
        status = report.Status.ToString(),
        totalDuration = report.TotalDuration.TotalMilliseconds,
        entries = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            description = entry.Value.Description,
            duration = entry.Value.Duration.TotalMilliseconds
        })
    };

    var json = System.Text.Json.JsonSerializer.Serialize(payload, new System.Text.Json.JsonSerializerOptions
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    });

    context.Response.StatusCode = report.Status ==
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy ? 200 : 503;

    await context.Response.WriteAsync(json);
}
