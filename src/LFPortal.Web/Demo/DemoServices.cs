using LFPortal.Application.DTOs;
using LFPortal.Application.Interfaces;
using LFPortal.Domain.Common;
using LFPortal.Domain.Entities;
using LFPortal.Infrastructure.Adapters;

namespace LFPortal.Web.Demo;

internal sealed class DemoDashboardService : ILaserficheDashboardService
{
    public Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(DemoDataStore.CreateDashboardStats());
}

internal sealed class DemoEntryService : ILaserficheEntryService
{
    public Task<LFEntry> GetEntryAsync(int entryId, CancellationToken cancellationToken = default) =>
        Task.FromResult(DemoDataStore.GetEntry(entryId));

    public Task<IReadOnlyList<LFFieldValue>> GetEntryFieldsAsync(int entryId, CancellationToken cancellationToken = default) =>
        Task.FromResult(DemoDataStore.GetFields(entryId));

    public Task<LFTemplate?> GetEntryTemplateAsync(int entryId, CancellationToken cancellationToken = default)
    {
        var entry = DemoDataStore.GetEntry(entryId);
        LFTemplate? template = string.IsNullOrWhiteSpace(entry.TemplateName)
            ? null
            : new LFTemplate
            {
                Id = entry.TemplateId ?? 500,
                Name = entry.TemplateName,
                Description = "Demo metadata template",
                Fields = DemoDataStore.FieldDefinitions.Values.ToList().AsReadOnly()
            };
        return Task.FromResult(template);
    }

    public Task<string> GetEntryPathAsync(int entryId, CancellationToken cancellationToken = default) =>
        Task.FromResult(DemoDataStore.GetEntry(entryId).FullPath);

    public Task<PagedResult<LFEntry>> GetEntryChildrenAsync(
        int entryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var all = DemoDataStore.GetChildren(entryId);
        var safePage = Math.Max(1, page);
        var safeSize = Math.Clamp(pageSize, 1, 100);
        var items = all.Skip((safePage - 1) * safeSize).Take(safeSize).ToList().AsReadOnly();
        return Task.FromResult(new PagedResult<LFEntry>
        {
            Items = items,
            TotalCount = all.Count,
            PageNumber = safePage,
            PageSize = safeSize
        });
    }

    public Task<int> GetRootEntryIdAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(DemoDataStore.RootEntryId);

    public Task<IReadOnlyList<LFEntry>> GetAllFolderChildrenAsync(int entryId, CancellationToken cancellationToken = default) =>
        Task.FromResult(DemoDataStore.GetChildren(entryId));

    public Task<IReadOnlyList<LFEntry>> GetFolderTreeAsync(int rootEntryId, int depth, CancellationToken cancellationToken = default) =>
        Task.FromResult(DemoDataStore.ArchiveFolders);
}

internal sealed class DemoFieldDefinitionService : ILaserficheFieldDefinitionService
{
    public Task<IReadOnlyDictionary<int, LFFieldDefinition>> GetFieldDefinitionsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(DemoDataStore.FieldDefinitions);
}

internal sealed class DemoRepositoryService : ILaserficheRepositoryService
{
    private static RepositoryInfo DemoRepository => new()
    {
        RepositoryId = DemoDataStore.RepositoryId,
        RepositoryName = DemoDataStore.RepositoryName,
        ServerVersion = "Demo Connected",
        ApiVersion = "Demo",
        SupportsAuthorizationCodeFlow = false
    };

    public Task<IReadOnlyList<RepositoryInfo>> DiscoverRepositoriesAsync(
        string serverUrl,
        string repositoryId,
        string username,
        string password,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<RepositoryInfo>>
        ([
            DemoRepository,
            new RepositoryInfo
            {
                RepositoryId = "DemoArchive",
                RepositoryName = "Demo Archive",
                ServerVersion = "Demo Connected",
                ApiVersion = "Demo",
                SupportsAuthorizationCodeFlow = false
            }
        ]);

    public Task<RepositoryInfo> GetRepositoryInfoAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(DemoRepository);

    public Task<ConnectionStatus> TestConnectionAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(ConnectionStatus.Success(DemoRepository));

    public Task<ConnectionStatus> TestConnectionWithCredentialsAsync(
        string serverUrl,
        string repositoryId,
        string username,
        string password,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(ConnectionStatus.Success(DemoRepository));
}

internal sealed class DemoCredentialProvider : ICredentialProvider
{
    public Task<LaserficheCredential> GetCredentialsAsync(string repositoryKey, CancellationToken cancellationToken = default) =>
        Task.FromException<LaserficheCredential>(new InvalidOperationException("Credentials are disabled in DemoMode."));

    public Task StoreCredentialsAsync(
        string repositoryKey,
        string username,
        string password,
        CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal sealed class DemoPortalConfigurationService : IPortalConfigurationService
{
    public Task SaveConnectionSettingsAsync(
        string serverUrl,
        string repositoryId,
        string displayName,
        string apiBasePath,
        string apiVersion,
        int rootEntryId,
        int timeoutSeconds,
        CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task SaveDetectedApiVersionAsync(string detectedVersion, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public bool HasSavedCredentials() => false;
    public bool HasEnvironmentVariableCredentials() => false;
}

internal sealed class DemoRepositoryContext : IRepositoryContext
{
    private static readonly RepositoryDescriptor Repository = new(
        "demo",
        DemoDataStore.ServerDisplayName,
        DemoDataStore.RepositoryId,
        DemoDataStore.RepositoryName);

    public Task<RepositoryDescriptor> GetActiveRepositoryAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Repository);

    public Task<IReadOnlyList<RepositoryDescriptor>> GetAllRepositoriesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<RepositoryDescriptor>>([Repository]);
}

internal sealed class DemoAuthService : ILaserficheAuthService
{
    public Task<string> GetTokenAsync(RepositoryDescriptor repository, CancellationToken cancellationToken = default) =>
        Task.FromException<string>(new InvalidOperationException("Laserfiche tokens are disabled in DemoMode."));

    public Task InvalidateTokenAsync(RepositoryDescriptor repository) => Task.CompletedTask;
    public Task InvalidateCurrentSessionTokensAsync() => Task.CompletedTask;

    public Task<bool> TryAuthenticateAsync(
        RepositoryDescriptor repository,
        string username,
        string password,
        CancellationToken cancellationToken = default) => Task.FromResult(false);

    public Task<bool> ExchangeAuthorizationCodeAsync(
        RepositoryDescriptor repository,
        string code,
        string codeVerifier,
        string redirectUri,
        string clientId,
        CancellationToken cancellationToken = default) => Task.FromResult(false);
}

internal sealed class DemoSessionCredentialStore : ISessionCredentialStore
{
    public Task StoreAsync(string username, string password, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task<LaserficheCredential?> TryGetAsync(CancellationToken cancellationToken = default) => Task.FromResult<LaserficheCredential?>(null);
    public Task ClearAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal sealed class DemoLaserficheApiAdapter : ILaserficheApiAdapter
{
    public string ApiVersion => "Demo";
    public string BuildRepositoriesUrl() => "demo://repositories";
    public string BuildRepositoriesUrlFor(string serverUrl) => "demo://repositories";
    public string BuildTokenUrl(string repositoryId) => "demo://disabled-token";
    public string BuildEntryUrl(string repositoryId, int entryId, EntryResource resource) => $"demo://entries/{entryId}";
    public string BuildPageImageUrl(string repositoryId, int entryId, int pageNumber) => $"demo://entries/{entryId}/pages/{pageNumber}";
    public string BuildSearchUrl(string repositoryId, SearchType searchType) => "demo://search";
    public string BuildTaskStatusUrl(string repositoryId, string operationToken) => "demo://task";
    public string BuildSearchResultsUrl(string repositoryId, string operationToken) => "demo://search-results";
    public string BuildTokenUrlFor(string serverUrl, string repositoryId) => "demo://disabled-token";
    public string BuildFolderChildrenUrl(string repositoryId, int entryId) => $"demo://entries/{entryId}/children";
    public string BuildTemplateDefinitionsUrl(string repositoryId) => "demo://templates";
    public string BuildFieldDefinitionsUrl(string repositoryId) => "demo://fields";
    public string BuildEntryByPathUrl(string repositoryId, string fullPath) => "demo://entry-by-path";
    public int GetConfiguredRootEntryId() => DemoDataStore.RootEntryId;
    public string BuildTokenUrlV2(string repositoryId) => "demo://disabled-token";
}
