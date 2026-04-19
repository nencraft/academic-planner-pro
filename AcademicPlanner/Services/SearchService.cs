using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Services;

public class SearchService
{
    private readonly AcademicPlannerDatabase _database;

    public SearchService(AcademicPlannerDatabase database)
    {
        _database = database;
    }

    public async Task<List<PlannerItem>> SearchAsync(string query)
    {
        query = query?.Trim() ?? string.Empty;

        var plannerItems = await BuildValidPlannerItemsAsync();

        if (string.IsNullOrWhiteSpace(query))
        {
            return plannerItems
                .OrderBy(i => i.StartDate)
                .ToList();
        }

        return plannerItems
            .Where(i =>
                i.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                i.ItemType.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                i.Subtitle.Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(i => i.StartDate)
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
}