namespace LFPortal.Web.Demo;

/// <summary>Safety switch for this design-only copy. It has no production backend path.</summary>
public sealed class DemoModeOptions
{
    public const string SectionName = "DemoMode";
    public bool Enabled { get; init; } = true;
    public bool BypassAuthentication { get; init; } = true;
    public bool UseMockData { get; init; } = true;
}
