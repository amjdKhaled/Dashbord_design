using LFPortal.Domain.Entities;
using LFPortal.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace LFPortal.Web.Demo;

/// <summary>
/// Keeps the existing Settings Razor page/controller intact while making its mutable
/// actions demo-safe. The filter runs only when DemoMode is enabled.
/// </summary>
internal sealed class DemoSettingsActionFilter : IAsyncActionFilter
{
    private readonly IOptions<DemoModeOptions> _demoMode;
    private readonly IModelMetadataProvider _metadataProvider;
    private readonly ILogger<DemoSettingsActionFilter> _logger;

    public DemoSettingsActionFilter(
        IOptions<DemoModeOptions> demoMode,
        IModelMetadataProvider metadataProvider,
        ILogger<DemoSettingsActionFilter> logger)
    {
        _demoMode = demoMode;
        _metadataProvider = metadataProvider;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!_demoMode.Value.Enabled ||
            !string.Equals(context.RouteData.Values["controller"]?.ToString(), "Settings", StringComparison.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        var action = context.RouteData.Values["action"]?.ToString() ?? string.Empty;

        if (HttpMethods.IsGet(context.HttpContext.Request.Method) &&
            string.Equals(action, nameof(SettingsController.Index), StringComparison.OrdinalIgnoreCase))
        {
            var saved = context.HttpContext.Request.Query.TryGetValue("saved", out var savedValue) &&
                        bool.TryParse(savedValue.ToString(), out var isSaved) && isSaved;
            var status = CreateDemoConnectionStatus();
            context.Result = new ViewResult
            {
                ViewName = "Index",
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<SettingsViewModel>(
                    _metadataProvider,
                    context.ModelState)
                {
                    Model = new SettingsViewModel
                    {
                        ServerUrl = DemoDataStore.ServerDisplayName,
                        RepositoryId = DemoDataStore.RepositoryId,
                        DisplayName = DemoDataStore.RepositoryName,
                        ApiBasePath = "/DemoApi",
                        ApiVersion = "Demo",
                        DetectedApiVersion = "Demo",
                        EffectiveApiVersion = "Demo",
                        IsAutoApiVersion = false,
                        RootEntryId = DemoDataStore.RootEntryId,
                        TimeoutSeconds = 30,
                        HasSavedCredentials = false,
                        HasEnvironmentVariableCredentials = false,
                        SaveSuccess = saved,
                        ConnectionStatus = status,
                        ActiveRepositoryId = DemoDataStore.RepositoryId,
                        ActiveRepositorySource = "Laserfiche Desktop Client",
                        AuthenticationMode = "Demo Mode",
                        DashboardPublicBaseUrl = string.Empty,
                        SsoLfdsBaseUrl = string.Empty,
                        SsoClientId = "Demo",
                        SsoCallbackUrl = string.Empty,
                        SsoAuthorizationEndpoint = string.Empty,
                        SsoTokenEndpoint = string.Empty
                    }
                }
            };
            return;
        }

        if (HttpMethods.IsPost(context.HttpContext.Request.Method))
        {
            if (string.Equals(action, nameof(SettingsController.Save), StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("DemoMode Settings Save: simulated success; no configuration or credentials were written.");
                context.Result = new RedirectToActionResult(nameof(SettingsController.Index), "Settings", new { saved = true });
                return;
            }

            if (string.Equals(action, nameof(SettingsController.TestConnection), StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("DemoMode TestConnection: simulated success; no server call was performed.");
                var status = CreateDemoConnectionStatus();
                context.Result = new PartialViewResult
                {
                    ViewName = "_TestResult",
                    ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<ConnectionStatus>(
                        _metadataProvider,
                        context.ModelState)
                    {
                        Model = status
                    }
                };
                return;
            }

            if (string.Equals(action, nameof(SettingsController.DiscoverRepositories), StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("DemoMode DiscoverRepositories: returning mock repositories only.");
                context.Result = new JsonResult(new
                {
                    repositories = new[]
                    {
                        new { id = DemoDataStore.RepositoryId, name = DemoDataStore.RepositoryName },
                        new { id = "DemoArchive", name = "Demo Archive" }
                    }
                });
                return;
            }
        }

        await next();
    }

    private static ConnectionStatus CreateDemoConnectionStatus() =>
        ConnectionStatus.Success(new RepositoryInfo
        {
            RepositoryId = DemoDataStore.RepositoryId,
            RepositoryName = DemoDataStore.RepositoryName,
            ServerVersion = "Demo Connected",
            ApiVersion = "Demo",
            SupportsAuthorizationCodeFlow = false
        });
}
