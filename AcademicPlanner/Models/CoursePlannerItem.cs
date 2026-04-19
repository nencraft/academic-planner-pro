using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlanner.Models;

public class CoursePlannerItem : PlannerItem
{
    public string StatusValue { get; set; } = string.Empty;

    public string InstructorName { get; set; } = string.Empty;

    public override string ItemType => "Course";

    public override string Subtitle => $"Instructor: {InstructorName} | Status: {StatusValue}";

    public override string NavigationRoute => $"{nameof(Views.CourseOverviewPage)}?courseId={SourceId}";

    public override ReportRow ToReportRow()
    {
        return new ReportRow
        {
            ItemType = ItemType,
            Title = Title,
            Context = InstructorName,
            StartDate = StartDate.ToString("MM/dd/yyyy"),
            EndDate = EndDate.ToString("MM/dd/yyyy"),
            Status = StatusValue
        };
    }
}