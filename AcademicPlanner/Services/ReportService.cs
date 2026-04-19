using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Services;

public class ReportService
{
    private readonly AcademicPlannerDatabase _database;

    public ReportService(AcademicPlannerDatabase database)
    {
        _database = database;
    }

    public async Task<List<ReportRow>> GetUpcomingAcademicActivityReportAsync()
    {
        var plannerItems = await BuildValidPlannerItemsAsync();

        DateTime today = DateTime.Today;

        return plannerItems
            .Where(i => i.EndDate.Date >= today) // keeps current and upcoming items
            .OrderBy(i => i.StartDate)
            .Select(i => i.ToReportRow())
            .ToList();
    }

    private async Task<List<PlannerItem>> BuildValidPlannerItemsAsync()
    {
        var terms = await _database.GetTermsAsync();
        var courses = await _database.GetAllCoursesAsync();
        var assessments = await _database.GetAllAssessmentsAsync();

        var validTermIds = terms
            .Select(t => t.Id)
            .ToHashSet();

        var validCourses = courses
            .Where(c => validTermIds.Contains(c.TermId))
            .ToList();

        var validCourseIds = validCourses
            .Select(c => c.Id)
            .ToHashSet();

        var validAssessments = assessments
            .Where(a => validCourseIds.Contains(a.CourseId))
            .ToList();

        var courseMap = validCourses.ToDictionary(c => c.Id, c => c.Title);

        var plannerItems = new List<PlannerItem>();

        plannerItems.AddRange(terms.Select(t => new TermPlannerItem
        {
            SourceId = t.Id,
            Title = t.Title,
            StartDate = t.StartDate,
            EndDate = t.EndDate
        }));

        plannerItems.AddRange(validCourses.Select(c => new CoursePlannerItem
        {
            SourceId = c.Id,
            Title = c.Title,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            StatusValue = c.Status,
            InstructorName = c.InstructorName
        }));

        plannerItems.AddRange(validAssessments.Select(a => new AssessmentPlannerItem
        {
            SourceId = a.Id,
            Title = a.Title,
            StartDate = a.StartDate,
            EndDate = a.EndDate,
            AssessmentType = a.Type,
            ParentCourseTitle = courseMap[a.CourseId]
        }));

        return plannerItems;
    }
    public async Task<string> ExportReportRowsToCsvAsync(IEnumerable<ReportRow> rows)
    {
        var safeRows = rows?.ToList() ?? new List<ReportRow>();

        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        string fileName = $"upcoming-academic-activity-report-{timestamp}.csv";
        string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        var sb = new StringBuilder();

        // header row
        sb.AppendLine("Type,Title,Context,Start Date,End/Due Date,Status");

        foreach (var row in safeRows)
        {
            sb.Append(EscapeCsv(row.ItemType)).Append(",");
            sb.Append(EscapeCsv(row.Title)).Append(",");
            sb.Append(EscapeCsv(row.Context)).Append(",");
            sb.Append(EscapeCsv(row.StartDate)).Append(",");
            sb.Append(EscapeCsv(row.EndDate)).Append(",");
            sb.Append(EscapeCsv(row.Status)).AppendLine();
        }

        await File.WriteAllTextAsync(filePath, sb.ToString());

        return filePath;
    }
    private static string EscapeCsv(string? value)
    {
        value ??= string.Empty;

        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }

        return value;
    }

}