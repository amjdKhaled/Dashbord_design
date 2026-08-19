using LFPortal.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LFPortal.Web.Controllers;

/// <summary>Offline archive browser backed exclusively by static demo entries.</summary>
public sealed class ArchiveController : Controller
{
    private static readonly string[] FolderNames = ["إدخال الوثيقة", "الأرشفة المباشرة للوثائق", "مركز الوثائق والمحفوظات", "الموارد البشرية", "المالية", "المشاريع", "العقود", "المراسلات"];
    private static readonly string[] DocumentNames = ["طلب اعتماد موظف.pdf", "فاتورة مشروع 1042.pdf", "خطاب رسمي.docx", "نموذج مراجعة.pdf", "تقرير شهري.pdf", "مستند أرشفة.pdf"];

    [HttpGet]
    public IActionResult Index(int entryId = 1, string trail = "")
    {
        var now = DateTimeOffset.UtcNow;
        var entries = FolderNames.Select((name, i) => new LFEntry { Id = 10 + i, ParentId = 1, Name = name, EntryType = LFEntryType.Folder, CreationTime = now.AddDays(-30-i), LastModifiedTime = now.AddDays(-i) })
            .Concat(DocumentNames.Select((name, i) => new LFEntry { Id = 100 + i, ParentId = 1, Name = name, EntryType = LFEntryType.Document, Creator = i % 2 == 0 ? "Demo User" : "سارة أحمد", CreationTime = now.AddDays(-i-2), LastModifiedTime = now.AddHours(-i-1), TemplateName = i % 2 == 0 ? "Employee" : "SASO (2)", FileSizeBytes = 180000 + i * 42000, PageCount = 2 + i })).ToArray();
        return View(new ArchiveViewModel { CurrentEntryId = 1, CurrentName = "TestEmployee", Entries = entries, IsConnected = true, Breadcrumb = [new BreadcrumbItem { EntryId = 1, Name = "TestEmployee" }] });
    }

    [HttpGet]
    public IActionResult Detail(int entryId)
    {
        var name = DocumentNames.ElementAtOrDefault(entryId - 100) ?? "مستند تجريبي.pdf";
        return PartialView("_EntryDetail", new ArchiveDetailViewModel { Entry = new LFEntry { Id = entryId, Name = name, EntryType = LFEntryType.Document, Creator = "Demo User", TemplateName = "Employee", CreationTime = DateTimeOffset.UtcNow.AddDays(-3), LastModifiedTime = DateTimeOffset.UtcNow.AddHours(-2), FileSizeBytes = 245000, PageCount = 4 }, Fields = [new LFFieldValue { FieldName = "الحالة", Value = "نسخة تجريبية" }, new LFFieldValue { FieldName = "القسم", Value = "الموارد البشرية" }] });
    }
}
