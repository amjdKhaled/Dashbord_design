using LFPortal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LFPortal.Web.Controllers;

/// <summary>Supplies the original Settings UI with read-only, in-process demo values.</summary>
public sealed class SettingsController : Controller
{
    [HttpGet]
    public IActionResult Index(bool saved = false) => View(SettingsViewModel.Create(saved));

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Save() => RedirectToAction(nameof(Index), new { saved = true });

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult TestConnection() => Json(new
    {
        success = true,
        repositoryName = "TestEmployee",
        serverVersion = "Demo Server",
        apiVersion = "Mock",
        message = "Demo Connected — no connection was attempted."
    });

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult DiscoverRepositories() => Json(new
    {
        repositories = new[] { new { id = "TestEmployee", name = "TestEmployee" } }
    });
}

public sealed class SettingsViewModel
{
    public string ServerUrl { get; init; } = "Demo Server";
    public string RepositoryId { get; init; } = "TestEmployee";
    public string DisplayName { get; init; } = "Offline Demo";
    public string ApiBasePath { get; init; } = "/MockData";
    public string ApiVersion { get; init; } = "Mock";
    public string DetectedApiVersion { get; init; } = "Mock";
    public string EffectiveApiVersion { get; init; } = "Mock";
    public bool IsAutoApiVersion { get; init; } = true;
    public int RootEntryId { get; init; } = 1;
    public int TimeoutSeconds { get; init; } = 0;
    public bool HasSavedCredentials { get; init; }
    public bool HasEnvironmentVariableCredentials { get; init; }
    public bool SaveSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public ConnectionStatus? ConnectionStatus { get; init; }
    public string? ActiveRepositoryId { get; init; } = "TestEmployee";
    public string? ActiveRepositorySource { get; init; } = "Laserfiche Desktop Client";
    public string EffectiveRepositoryId => ActiveRepositoryId ?? RepositoryId;
    public string EffectiveRepositorySource => ActiveRepositorySource ?? "Offline Demo";
    public string AuthenticationMode { get; init; } = "Demo Mode — Bypassed";
    public string SsoLfdsBaseUrl { get; init; } = string.Empty;
    public string SsoClientId { get; init; } = "Disabled";
    public string SsoCallbackUrl { get; init; } = string.Empty;
    public string SsoAuthorizationEndpoint { get; init; } = string.Empty;
    public string SsoTokenEndpoint { get; init; } = string.Empty;
    public string DashboardPublicBaseUrl { get; init; } = "Offline Demo";
    public bool SsoEnabled => false;
    public bool SsoConfigurationValid => false;

    public static SettingsViewModel Create(bool saved) => new()
    {
        SaveSuccess = saved,
        ConnectionStatus = new ConnectionStatus
        {
            IsConnected = true,
            RepositoryFound = true,
            RepositoryId = "TestEmployee",
            RepositoryName = "TestEmployee",
            ServerVersion = "Demo Server",
            ApiVersion = "Mock",
            CheckedAt = DateTimeOffset.UtcNow
        }
    };
}
