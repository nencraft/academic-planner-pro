using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlanner.Models;

public abstract class PlannerItem
{
    public int SourceId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public abstract string ItemType { get; }

    public abstract string Subtitle { get; }

    public abstract string NavigationRoute { get; }

    public virtual ReportRow ToReportRow()
    {
        return new ReportRow
        {
            ItemType = ItemType,
            Title = Title,
            Context = Subtitle,
            StartDate = StartDate.ToString("MM/dd/yyyy"),
            EndDate = EndDate.ToString("MM/dd/yyyy"),
            Status = string.Empty
        };
    }
}
