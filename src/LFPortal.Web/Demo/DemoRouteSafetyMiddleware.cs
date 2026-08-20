namespace LFPortal.Web.Demo;

internal sealed class DemoRouteSafetyMiddleware
{
    private readonly RequestDelegate _next;

    public DemoRouteSafetyMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Keep the normal demo pages and their mock-only actions reachable.
        var path = context.Request.Path;

        if (path.StartsWithSegments("/Login") ||
            path.StartsWithSegments("/Launch"))
        {
            context.Response.Redirect("/Dashboard");
            return;
        }

        // Probe is now a mock-only page and is intentionally reachable.
        // Real document/share/API routes remain blocked in this standalone demo.
        if (path.StartsWithSegments("/Document") ||
            path.StartsWithSegments("/Share") ||
            path.StartsWithSegments("/LaserficheApi") ||
            path.StartsWithSegments("/api"))
        {
            context.Response.Redirect("/Dashboard");
            return;
        }

        context.Session.SetString("ActiveRepositoryId", DemoDataStore.RepositoryId);
        context.Session.SetString("ActiveRepositorySource", "Laserfiche Desktop Client");

        await _next(context);
    }
}
