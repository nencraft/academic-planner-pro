using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Data;
using AcademicPlanner.Models;

namespace AcademicPlanner.Services;

public class SeedDataService
{
    private readonly AcademicPlannerDatabase _database;

    public SeedDataService(AcademicPlannerDatabase database)
    {
        _database = database;
    }

    public async Task SeedAsync()
    {
        await SeedLookupOptionsAsync();

        if (await _database.HasAnyDataAsync())
            return;

        Term term = new()
        {
            Title = "Term 1",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 6, 30)
        };

        await _database.SaveTermAsync(term);

        Course course = new()
        {
            TermId = term.Id,
            Title = "Mobile Application Development",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 2, 1),
            Status = "In Progress",
            InstructorName = "Anika Patel",
            InstructorPhone = "555-123-4567",
            InstructorEmail = "anika.patel@strimeuniversity.edu",
            Notes = "Seeded evaluation course data.",
            AlertSetting = "None"
        };

        await _database.SaveCourseAsync(course);

        Assessment objectiveAssessment = new()
        {
            CourseId = course.Id,
            Title = "Objective Assessment",
            Type = "Objective",
            StartDate = new DateTime(2026, 1, 10),
            EndDate = new DateTime(2026, 1, 20),
            AlertSetting = "None"
        };

        Assessment performanceAssessment = new()
        {
            CourseId = course.Id,
            Title = "Performance Assessment",
            Type = "Performance",
            StartDate = new DateTime(2026, 1, 21),
            EndDate = new DateTime(2026, 2, 1),
            AlertSetting = "None"
        };

        await _database.SaveAssessmentAsync(objectiveAssessment);
        await _database.SaveAssessmentAsync(performanceAssessment);
    }

    private async Task SeedLookupOptionsAsync()
    {
        await _database.EnsureCourseStatusOptionAsync("In Progress", 1);
        await _database.EnsureCourseStatusOptionAsync("Completed", 2);
        await _database.EnsureCourseStatusOptionAsync("Dropped", 3);
        await _database.EnsureCourseStatusOptionAsync("Plan to Take", 4);

        await _database.EnsureAlertOptionAsync("None", 1);
        await _database.EnsureAlertOptionAsync("Start", 2);
        await _database.EnsureAlertOptionAsync("End", 3);
        await _database.EnsureAlertOptionAsync("Start and End", 4);
    }
}
