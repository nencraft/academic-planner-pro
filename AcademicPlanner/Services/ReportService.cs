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
        var terms = await _database.GetTermsAsync();
        var courses = await _database.GetAllCoursesAsync();
        var assessments = await _database.GetAllAssessmentsAsync();

        var courseMap = courses.ToDictionary(c => c.Id, c => c.Title);

        var plannerItems = new List<PlannerItem>();

        plannerItems.AddRange(terms.Select(t => new TermPlannerItem
        {
            SourceId = t.Id,
            Title = t.Title,
            StartDate = t.StartDate,
            EndDate = t.EndDate
        }));

        plannerItems.AddRange(courses.Select(c => new CoursePlannerItem
        {
            SourceId = c.Id,
            Title = c.Title,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            StatusValue = c.Status,
            InstructorName = c.InstructorName
        }));

        plannerItems.AddRange(assessments.Select(a => new AssessmentPlannerItem
        {
            SourceId = a.Id,
            Title = a.Title,
            StartDate = a.StartDate,
            EndDate = a.EndDate,
            AssessmentType = a.Type,
            ParentCourseTitle = courseMap.TryGetValue(a.CourseId, out var courseTitle)
                ? courseTitle
                : "Unknown Course"
        }));

        return plannerItems
            .OrderBy(i => i.StartDate)
            .Select(i => i.ToReportRow())
            .ToList();
    }
}
