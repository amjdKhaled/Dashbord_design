using LFPortal.Domain.Entities;
using LFPortal.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LFPortal.Web.Demo;

/// <summary>
/// Keeps the existing Settings Razor page/controller intact while making every mutable
/// action permanently demo-safe. This repository is a standalone internet demo only.
/// </summary>
internal sealed class DemoSettingsActionFilter : IAsyncActionFilter
{
    private readonly IModelMetadataProvider _metadataProvider;
    private readonly ILogger<DemoSettingsActionFilter> _logger;

    public DemoSettingsActionFilter(
        IModelMetadataProvider metadataProvider,
        ILogger<DemoSettingsActionFilter> logger)
    {
        _metadataProvider = metadataProvider;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!string.Equals(
                context.RouteData.Values["controller"]?.ToString(),
                "Settings",
                StringComparison.OrdinalIgnoreCase))
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
                        ConnectionStatus = CreateDemoConnectionStatus(),
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
                _logger.LogInformation("Demo Settings Save: simulated success; nothing was written to disk or a server.");
                context.Result = new RedirectToActionResult(
                    nameof(SettingsController.Index),
                    "Settings",
                    new { saved = true });
                return;
            }

            if (string.Equals(action, nameof(SettingsController.TestConnection), StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Demo TestConnection: simulated success; no network request was made.");
                context.Result = new PartialViewResult
                {
                    ViewName = "_TestResult",
                    ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<ConnectionStatus>(
                        _metadataProvider,
                        context.ModelState)
                    {
                        Model = CreateDemoConnectionStatus()
                    }
                };
                return;
            }

            if (string.Equals(action, nameof(SettingsController.DiscoverRepositories), StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Demo DiscoverRepositories: returning hardcoded repositories only.");
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
