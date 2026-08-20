using LFPortal.Application.DTOs;
using LFPortal.Application.Interfaces;
using LFPortal.Web.Demo;
using Microsoft.AspNetCore.Mvc;

namespace LFPortal.Web.Controllers;

/// <summary>
/// Displays the standalone demo dashboard using hardcoded Laserfiche-like data.
/// </summary>
public sealed class DashboardController : Controller
{
    private readonly ILaserficheDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        ILaserficheDashboardService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Dashboard demo: fetching hardcoded statistics.");
        var stats = await _dashboardService.GetDashboardStatsAsync(cancellationToken);
        _logger.LogInformation(
            "Dashboard demo stats: connected={Connected}, docs={Docs}, folders={Folders}, templates={Templates}.",
            stats.IsConnected, stats.TotalDocuments, stats.TotalFolders, stats.TotalTemplates);

        return View(new DashboardViewModel { Stats = stats });
    }

    /// <summary>
    /// Keeps the existing Probe view available in the internet demo without making
    /// any real HTTP/API calls. The design stays unchanged; only the values are mock.
    /// </summary>
    [HttpGet("/Dashboard/Probe")]
    public IActionResult Probe()
    {
        var probes = new List<ProbeResult>
        {
            new()
            {
                Label = "GET /Repositories",
                Url = "demo://repositories",
                StatusCode = 200,
                Status = "OK",
                IsSuccess = true,
                ContentType = "application/json",
                Body = "{\"value\":[{\"id\":\"TestEmployee\",\"name\":\"TestEmployee\"}]}",
                ElapsedMs = 3
            },
            new()
            {
                Label = "GET /Entries/1 (root entry details)",
                Url = "demo://repositories/TestEmployee/entries/1",
                StatusCode = 200,
                Status = "OK",
                IsSuccess = true,
                ContentType = "application/json",
                Body = "{\"id\":1,\"name\":\"Repository\",\"entryType\":\"Folder\"}",
                ElapsedMs = 2
            },
            new()
            {
                Label = "GET /Entries/1/Folder/Children",
                Url = "demo://repositories/TestEmployee/entries/1/children",
                StatusCode = 200,
                Status = "OK",
                IsSuccess = true,
                ContentType = "application/json",
                Body = "{\"folders\":8,\"documents\":6,\"source\":\"hardcoded demo data\"}",
                ElapsedMs = 4
            },
            new()
            {
                Label = "GET /TemplateDefinitions",
                Url = "demo://repositories/TestEmployee/templates",
                StatusCode = 200,
                Status = "OK",
                IsSuccess = true,
                ContentType = "application/json",
                Body = "{\"count\":22,\"source\":\"hardcoded demo data\"}",
                ElapsedMs = 2
            }
        };

        return View("Probe", new ProbeViewModel
        {
            ServerUrl = DemoDataStore.ServerDisplayName,
            RepositoryId = DemoDataStore.RepositoryId,
            Username = "Demo User",
            RootEntryId = DemoDataStore.RootEntryId,
            RootDiscoveryNote = "Hardcoded demo root entry",
            Probes = probes
        });
    }
}

public sealed class DashboardViewModel
{
    public DashboardStatsDto Stats { get; init; } = new();
}

public sealed class ProbeViewModel
{
    public string ServerUrl { get; init; } = "";
    public string RepositoryId { get; init; } = "";
    public string? Username { get; init; }
    public string? CredError { get; init; }
    public int RootEntryId { get; init; } = 1;
    public string RootDiscoveryNote { get; init; } = "";
    public List<ProbeResult> Probes { get; init; } = [];
}

public sealed class ProbeResult
{
    public string Label { get; init; } = "";
    public string Url { get; init; } = "";
    public int StatusCode { get; init; }
    public string Status { get; init; } = "";
    public bool IsSuccess { get; init; }
    public string ContentType { get; init; } = "";
    public string Body { get; init; } = "";
    public long ElapsedMs { get; init; }
}
