using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlanner.Models;

public class AssessmentPlannerItem : PlannerItem
{
    public string AssessmentType { get; set; } = string.Empty;

    public string ParentCourseTitle { get; set; } = string.Empty;

    public override string ItemType => "Assessment";

    public override string Subtitle => $"{AssessmentType} | Course: {ParentCourseTitle}";

    public override string NavigationRoute => $"{nameof(Views.AssessmentEditPage)}?assessmentId={SourceId}";

    public override ReportRow ToReportRow()
    {
        return new ReportRow
        {
            ItemType = ItemType,
            Title = Title,
            Context = ParentCourseTitle,
            StartDate = StartDate.ToString("MM/dd/yyyy"),
            EndDate = EndDate.ToString("MM/dd/yyyy"),
            Status = AssessmentType
        };
    }
}