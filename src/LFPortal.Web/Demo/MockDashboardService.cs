using LFPortal.Application.DTOs;
using LFPortal.Application.Interfaces;
using LFPortal.Domain.Entities;

namespace LFPortal.Web.Demo;

/// <summary>Deterministic, in-process dashboard data. This class never performs I/O.</summary>
public sealed class MockDashboardService : ILaserficheDashboardService
{
    public Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var names = new[] { "طلب اعتماد موظف.pdf", "فاتورة مشروع 1042.pdf", "خطاب رسمي.docx", "نموذج مراجعة.pdf", "تقرير شهري.pdf", "مستند أرشفة.pdf" };
        var users = new[] { "Demo User", "سارة أحمد", "محمد علي", "Archive Demo" };
        var docs = Enumerable.Range(0, 56).Select(i => new LFEntry
        {
            Id = 1000 + i, Name = names[i % names.Length], EntryType = LFEntryType.Document,
            FullPath = $"/مركز الوثائق والمحفوظات/{names[i % names.Length]}",
            FolderPath = i % 2 == 0 ? "مركز الوثائق والمحفوظات" : "إدخال الوثيقة",
            Creator = users[i % users.Length], CreationTime = now.AddDays(-(i % 25)),
            LastModifiedTime = now.AddHours(-(i * 3 + 1)), TemplateName = "Employee",
            FileSizeBytes = 120_000 + i * 2100, PageCount = 1 + i % 8
        }).ToList().AsReadOnly();

        var result = new DashboardStatsDto
        {
            IsConnected = true, RepositoryId = "TestEmployee", RepositoryName = "TestEmployee",
            ServerVersion = "DESKTOP · Offline Demo", ServerUrl = "Offline mock data",
            ConnectedUser = "Demo User", AuthenticationMode = "Demo Mode (bypassed)",
            TotalFolders = 56, TotalDocuments = 56, TotalTemplates = 22,
            DocsWithTemplate = 56, DocsWithoutTemplate = 0,
            RootFolders = new[]
            {
                new RootFolderStatDto { Name = "إدخال الوثيقة", Documents = 21, Folders = 18 },
                new RootFolderStatDto { Name = "الأرشفة المباشرة للوثائق", Documents = 7, Folders = 12 },
                new RootFolderStatDto { Name = "مركز الوثائق والمحفوظات", Documents = 28, Folders = 26 }
            },
            TemplateStats = new[] { "SASO (2)", "SASO (5)", "Employee", "الأرشفة المباشرة للوثائق", "Template #0", "SASO (6)", "SASO (4)", "red" }
                .Select((name, i) => new TemplateStatDto { Name = name, Count = new[] { 11, 9, 8, 7, 6, 6, 5, 4 }[i] }).ToArray(),
            AllDocs = docs, RecentDocs = docs.Take(12).ToArray(), ModifiedDocs = docs.Take(12).ToArray(),
            LastCheckedAt = now, ScanDurationMs = 0
        };
        return Task.FromResult(result);
    }
}
