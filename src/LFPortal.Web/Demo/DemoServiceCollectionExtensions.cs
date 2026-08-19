using LFPortal.Application.Interfaces;
using LFPortal.Infrastructure.Adapters;
using LFPortal.Infrastructure.Options;
using Microsoft.Extensions.DependencyInjection;

namespace LFPortal.Web.Demo;

internal static class DemoServiceCollectionExtensions
{
    public static IServiceCollection AddDashboardDemoMode(this IServiceCollection services)
    {
        services.Configure<LaserficheOptions>(options =>
        {
            options.ServerUrl = DemoDataStore.ServerDisplayName;
            options.RepositoryId = DemoDataStore.RepositoryId;
            options.DisplayName = DemoDataStore.RepositoryName;
            options.ApiBasePath = "/DemoApi";
            options.ApiVersion = "Demo";
            options.DetectedApiVersion = "Demo";
            options.RootEntryId = DemoDataStore.RootEntryId;
            options.TimeoutSeconds = 30;
            options.DashboardPublicBaseUrl = string.Empty;
            options.Sso.LfdsBaseUrl = string.Empty;
            options.Sso.RedirectUri = string.Empty;
        });

        services.AddSingleton<ILaserficheDashboardService, DemoDashboardService>();
        services.AddSingleton<ILaserficheEntryService, DemoEntryService>();
        services.AddSingleton<ILaserficheFieldDefinitionService, DemoFieldDefinitionService>();
        services.AddSingleton<ILaserficheRepositoryService, DemoRepositoryService>();
        services.AddSingleton<ICredentialProvider, DemoCredentialProvider>();
        services.AddSingleton<IPortalConfigurationService, DemoPortalConfigurationService>();
        services.AddSingleton<IRepositoryContext, DemoRepositoryContext>();
        services.AddSingleton<ILaserficheAuthService, DemoAuthService>();
        services.AddSingleton<ISessionCredentialStore, DemoSessionCredentialStore>();
        services.AddSingleton<ILaserficheApiAdapter, DemoLaserficheApiAdapter>();

        services.AddHealthChecks()
            .AddCheck("demo", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(
                "DemoMode is enabled; Laserfiche connectivity is intentionally disabled."));

        return services;
    }
}
