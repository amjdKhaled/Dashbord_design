using LFPortal.Application.DTOs;
using LFPortal.Domain.Entities;

namespace LFPortal.Web.Demo;

internal static class DemoDataStore
{
    public const int RootEntryId = 1;
    public const string RepositoryId = "TestEmployee";
    public const string RepositoryName = "TestEmployee";
    public const string ServerDisplayName = "Demo Server";

    private static readonly string[] DemoUsers =
    [
        "Demo User",
        "سارة أحمد",
        "محمد علي",
        "Archive Demo"
    ];

    private static readonly string[] DocumentNames =
    [
        "طلب اعتماد موظف.pdf",
        "فاتورة مشروع 1042.pdf",
        "خطاب رسمي.docx",
        "نموذج مراجعة.pdf",
        "تقرير شهري.pdf",
        "مستند أرشفة.pdf"
    ];

    private static readonly string[] TemplateNames =
    [
        "Employee",
        "SASO (2)",
        "SASO (5)",
        "الأرشفة المباشرة للوثائق"
    ];

    public static IReadOnlyList<LFEntry> ArchiveFolders { get; } =
    [
        Folder(100, "إدخال الوثيقة"),
        Folder(101, "الأرشفة المباشرة للوثائق"),
        Folder(102, "مركز الوثائق والمحفوظات"),
        Folder(103, "الموارد البشرية"),
        Folder(104, "المالية"),
        Folder(105, "المشاريع"),
        Folder(106, "العقود"),
        Folder(107, "المراسلات")
    ];

    private static readonly IReadOnlyList<LFEntry> RootDocuments =
    [
        Document(9001, RootEntryId, DocumentNames[0], "Employee", "Demo User", 3, 640_000, 1),
        Document(9002, RootEntryId, DocumentNames[1], "SASO (2)", "سارة أحمد", 2, 1_480_000, 2),
        Document(9003, RootEntryId, DocumentNames[2], "SASO (5)", "محمد علي", 1, 285_000, 3),
        Document(9004, RootEntryId, DocumentNames[3], "Employee", "Archive Demo", 5, 920_000, 4),
        Document(9005, RootEntryId, DocumentNames[4], "SASO (2)", "Demo User", 8, 2_100_000, 5),
        Document(9006, RootEntryId, DocumentNames[5], "SASO (5)", "سارة أحمد", 4, 760_000, 6)
    ];

    private static readonly IReadOnlyDictionary<int, IReadOnlyList<LFEntry>> ChildrenByFolder =
        BuildChildren();

    private static readonly IReadOnlyDictionary<int, LFEntry> EntriesById = BuildEntryIndex();

    public static IReadOnlyDictionary<int, LFFieldDefinition> FieldDefinitions { get; } =
        new Dictionary<int, LFFieldDefinition>
        {
            [1] = new() { Id = 1, Name = "Template", FieldType = "String" },
            [2] = new() { Id = 2, Name = "Created by", FieldType = "String" },
            [3] = new() { Id = 3, Name = "Department", FieldType = "String" },
            [4] = new() { Id = 4, Name = "Status", FieldType = "String" }
        };

    public static DashboardStatsDto CreateDashboardStats()
    {
        var now = DateTimeOffset.UtcNow;
        var allDocs = Enumerable.Range(1, 56)
            .Select(i => new LFEntry
            {
                Id = 20_000 + i,
                Name = i <= DocumentNames.Length
                    ? DocumentNames[i - 1]
                    : $"Demo Document {i:00}.pdf",
                ParentId = 100 + (i % 3),
                FullPath = $"\\{ArchiveFolders[i % ArchiveFolders.Count].Name}\\Demo Document {i:00}.pdf",
                FolderPath = $"\\{ArchiveFolders[i % ArchiveFolders.Count].Name}",
                Creator = DemoUsers[(i - 1) % DemoUsers.Length],
                CreationTime = now.AddDays(-(i % 18)).AddMinutes(-i * 7),
                LastModifiedTime = now.AddHours(-(i % 72)).AddMinutes(-i * 3),
                EntryType = LFEntryType.Document,
                TemplateName = (i % 8) switch
                {
                    0 => "SASO (2)",
                    1 => "SASO (5)",
                    2 => "Employee",
                    3 => "الأرشفة المباشرة للوثائق",
                    4 => "Template #0",
                    5 => "SASO (6)",
                    6 => "SASO (4)",
                    _ => "red"
                },
                PageCount = 1 + (i % 12),
                FileSizeBytes = 180_000 + (i * 41_000L)
            })
            .ToList()
            .AsReadOnly();

        var recent = allDocs.OrderByDescending(x => x.CreationTime).Take(12).ToList().AsReadOnly();
        var modified = allDocs.OrderByDescending(x => x.LastModifiedTime).Take(12).ToList().AsReadOnly();

        return new DashboardStatsDto
        {
            IsConnected = true,
            RepositoryId = RepositoryId,
            RepositoryName = RepositoryName,
            ServerVersion = "Demo Connected",
            ServerUrl = ServerDisplayName,
            ConnectedUser = "Demo User",
            AuthenticationMode = "Demo Mode",
            TotalFolders = 56,
            TotalDocuments = 56,
            TotalTemplates = 22,
            DocsWithTemplate = 56,
            DocsWithoutTemplate = 0,
            RootFolders =
            [
                new RootFolderStatDto { Name = "إدخال الوثيقة", Documents = 21, Folders = 18 },
                new RootFolderStatDto { Name = "الأرشفة المباشرة للوثائق", Documents = 7, Folders = 12 },
                new RootFolderStatDto { Name = "مركز الوثائق والمحفوظات", Documents = 28, Folders = 26 }
            ],
            TemplateStats =
            [
                new TemplateStatDto { Name = "SASO (2)", Count = 10 },
                new TemplateStatDto { Name = "SASO (5)", Count = 9 },
                new TemplateStatDto { Name = "Employee", Count = 8 },
                new TemplateStatDto { Name = "الأرشفة المباشرة للوثائق", Count = 7 },
                new TemplateStatDto { Name = "Template #0", Count = 6 },
                new TemplateStatDto { Name = "SASO (6)", Count = 6 },
                new TemplateStatDto { Name = "SASO (4)", Count = 5 },
                new TemplateStatDto { Name = "red", Count = 5 }
            ],
            RecentDocs = recent,
            ModifiedDocs = modified,
            AllDocs = allDocs,
            ScanDurationMs = 42,
            LastCheckedAt = now,
            AvgSearchResponseTime = TimeSpan.FromMilliseconds(18),
            EntryTypeBreakdown = new Dictionary<string, int>
            {
                ["Document"] = 56,
                ["Folder"] = 56
            },
            DepartmentCount = 3,
            RecentDocuments = recent,
            RecentlyIndexedDocuments = modified,
            RecentEntries = modified
        };
    }

    public static LFEntry GetEntry(int entryId) => EntriesById.TryGetValue(entryId, out var entry)
        ? entry
        : throw new KeyNotFoundException($"Demo entry {entryId} was not found.");

    public static IReadOnlyList<LFEntry> GetChildren(int parentId) =>
        ChildrenByFolder.TryGetValue(parentId, out var children) ? children : [];

    public static IReadOnlyList<LFFieldValue> GetFields(int entryId)
    {
        var entry = GetEntry(entryId);
        if (entry.EntryType != LFEntryType.Document)
            return [];

        var department = entry.ParentId switch
        {
            103 => "الموارد البشرية",
            104 => "المالية",
            105 => "المشاريع",
            106 => "العقود",
            107 => "المراسلات",
            _ => "مركز الوثائق والمحفوظات"
        };

        return
        [
            new LFFieldValue { FieldDefinitionId = 1, FieldName = "Template", Value = entry.TemplateName, FieldType = "String" },
            new LFFieldValue { FieldDefinitionId = 2, FieldName = "Created by", Value = entry.Creator, FieldType = "String" },
            new LFFieldValue { FieldDefinitionId = 3, FieldName = "Department", Value = department, FieldType = "String" },
            new LFFieldValue { FieldDefinitionId = 4, FieldName = "Status", Value = "Demo Record", FieldType = "String" }
        ];
    }

    private static IReadOnlyDictionary<int, IReadOnlyList<LFEntry>> BuildChildren()
    {
        var result = new Dictionary<int, IReadOnlyList<LFEntry>>
        {
            [RootEntryId] = ArchiveFolders.Concat(RootDocuments).ToList().AsReadOnly()
        };

        for (var i = 0; i < ArchiveFolders.Count; i++)
        {
            var folder = ArchiveFolders[i];
            var name = DocumentNames[i % DocumentNames.Length];
            var template = TemplateNames[i % TemplateNames.Length];
            var user = DemoUsers[i % DemoUsers.Length];
            result[folder.Id] =
            [
                Document(9100 + i, folder.Id, name, template, user, 1 + (i % 9), 420_000 + i * 125_000L, i + 2)
            ];
        }

        return result;
    }

    private static IReadOnlyDictionary<int, LFEntry> BuildEntryIndex()
    {
        var root = new LFEntry
        {
            Id = RootEntryId,
            Name = "Repository",
            ParentId = 0,
            FullPath = "\\",
            FolderPath = "\\",
            EntryType = LFEntryType.Folder,
            Creator = "Demo User",
            CreationTime = DateTimeOffset.UtcNow.AddYears(-1),
            LastModifiedTime = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var all = new List<LFEntry> { root };
        all.AddRange(ArchiveFolders);
        foreach (var children in ChildrenByFolder.Values)
            all.AddRange(children);

        return all.GroupBy(x => x.Id).ToDictionary(g => g.Key, g => g.First());
    }

    private static LFEntry Folder(int id, string name) => new()
    {
        Id = id,
        Name = name,
        ParentId = RootEntryId,
        FullPath = $"\\{name}",
        FolderPath = "\\",
        Creator = "Archive Demo",
        CreationTime = DateTimeOffset.UtcNow.AddMonths(-6).AddDays(id % 20),
        LastModifiedTime = DateTimeOffset.UtcNow.AddDays(-(id % 10)),
        EntryType = LFEntryType.Folder
    };

    private static LFEntry Document(
        int id,
        int parentId,
        string name,
        string template,
        string creator,
        int pages,
        long size,
        int ageDays)
    {
        var folder = ArchiveFolders.FirstOrDefault(x => x.Id == parentId)?.Name;
        var pathPrefix = string.IsNullOrWhiteSpace(folder) ? "" : $"\\{folder}";
        return new LFEntry
        {
            Id = id,
            Name = name,
            ParentId = parentId,
            FullPath = $"{pathPrefix}\\{name}",
            FolderPath = string.IsNullOrWhiteSpace(pathPrefix) ? "\\" : pathPrefix,
            Creator = creator,
            CreationTime = DateTimeOffset.UtcNow.AddDays(-ageDays).AddHours(-2),
            LastModifiedTime = DateTimeOffset.UtcNow.AddDays(-Math.Max(0, ageDays - 1)).AddHours(-1),
            EntryType = LFEntryType.Document,
            TemplateName = template,
            TemplateId = 500 + (id % 8),
            PageCount = pages,
            FileSizeBytes = size
        };
    }
}
