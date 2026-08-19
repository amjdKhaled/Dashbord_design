using LFPortal.Web.Demo;
using Xunit;
namespace LFPortal.Web.Tests;

public sealed class DemoModeTests
{
    [Fact]
    public async Task Mock_dashboard_returns_required_values_without_credentials()
    {
        var stats = await new MockDashboardService().GetDashboardStatsAsync();
        Assert.True(stats.IsConnected);
        Assert.Equal("TestEmployee", stats.RepositoryName);
        Assert.Equal(56, stats.TotalDocuments);
        Assert.Equal(56, stats.TotalFolders);
        Assert.Equal(22, stats.TotalTemplates);
        Assert.Equal("Demo Mode (bypassed)", stats.AuthenticationMode);
    }

    [Fact]
    public void Demo_mode_defaults_are_safe()
    {
        var options = new DemoModeOptions();
        Assert.True(options.Enabled);
        Assert.True(options.BypassAuthentication);
        Assert.True(options.UseMockData);
    }
}
