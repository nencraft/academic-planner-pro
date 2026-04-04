using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace AcademicPlanner.Models;

public class Assessment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int CourseId { get; set; }

    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Type { get; set; } = string.Empty;

    public DateTime StartDate { get; set; } = DateTime.Today;

    public DateTime EndDate { get; set; } = DateTime.Today;

    [MaxLength(20)]
    public string AlertSetting { get; set; } = "None";

    public int? StartNotificationId { get; set; }

    public int? EndNotificationId { get; set; }

    [Ignore]
    public string BellIconSource =>
    AlertSetting == "None"
        ? "icon_bell_inactive.png"
        : "icon_bell_active.png";
}
