using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlanner.Models;

public class ReportRow
{
    public string ItemType { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Context { get; set; } = string.Empty;

    public string StartDate { get; set; } = string.Empty;

    public string EndDate { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}
