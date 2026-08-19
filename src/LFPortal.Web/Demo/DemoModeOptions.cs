namespace LFPortal.Web.Demo;

public sealed class DemoModeOptions
{
    public const string SectionName = "DemoMode";

    public bool Enabled { get; set; }
    public bool BypassAuthentication { get; set; } = true;
    public bool UseMockData { get; set; } = true;
}
