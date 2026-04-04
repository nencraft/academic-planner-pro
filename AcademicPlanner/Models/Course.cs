using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlanner.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int TermId { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Today;

        public DateTime EndDate { get; set; } = DateTime.Today;

        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [MaxLength(100)]
        public string InstructorName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string InstructorPhone { get; set; } = string.Empty;

        [MaxLength(100)]
        public string InstructorEmail { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        [MaxLength(20)]
        public string AlertSetting { get; set; } = "None";

        public int? StartNotificationId { get; set; }

        public int? EndNotificationId { get; set; }
    }
}
