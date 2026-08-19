using Microsoft.AspNetCore.Mvc;

namespace LFPortal.Web.Controllers;

public sealed class SettingsController : Controller
{
    [HttpGet]
    public IActionResult Index(bool saved = false) => View(new SettingsViewModel { SaveSuccess = saved });

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Save() => RedirectToAction(nameof(Index), new { saved = true });

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult TestConnection() => Content("Demo Connected — no connection was attempted.", "text/plain");
}

public sealed class SettingsViewModel { public bool SaveSuccess { get; init; } }
