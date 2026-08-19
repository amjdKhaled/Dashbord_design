using Microsoft.AspNetCore.Mvc;
namespace LFPortal.Web.Controllers;

/// <summary>Safe replacements for integration-era routes retained in bookmarks.</summary>
public sealed class DemoRoutesController : Controller
{
    [HttpGet("/"), HttpGet("/Login"), HttpGet("/Login/{*path}"), HttpGet("/Launch/{*path}")]
    public IActionResult DashboardRoute() => Redirect("/Dashboard");

    [HttpGet("/Diagnostics"), HttpGet("/Diagnostics/{*path}"), HttpGet("/api/diagnostics/{*path}")]
    public IActionResult Diagnostics() => Json(new { app = "Healthy", dataSource = "Mock", api = "Disabled in Demo Mode", authentication = "Bypassed" });

    [Route("/api/{*path}"), Route("/Share/{*path}"), Route("/Document/{*path}")]
    public IActionResult DisabledIntegration() => StatusCode(StatusCodes.Status410Gone, new { message = "Disabled in offline demo mode" });
}
