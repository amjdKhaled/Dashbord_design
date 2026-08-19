using LFPortal.Application.DTOs;
using LFPortal.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace LFPortal.Web.Controllers;

public sealed class DashboardController(ILaserficheDashboardService dashboardService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(new DashboardViewModel { Stats = await dashboardService.GetDashboardStatsAsync(cancellationToken) });

    [HttpGet("/Dashboard/Probe")]
    public IActionResult Probe() => RedirectToAction(nameof(Index));
}

public sealed class DashboardViewModel
{
    public DashboardStatsDto Stats { get; init; } = new();
}
