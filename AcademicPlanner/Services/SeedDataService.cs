using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademicPlanner.Data;

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