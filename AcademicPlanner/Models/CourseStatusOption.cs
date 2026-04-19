using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace AcademicPlanner.Models;

public class CourseStatusOption
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(50), Unique]
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }
}