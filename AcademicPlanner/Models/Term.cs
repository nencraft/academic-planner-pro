using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicPlanner.Models
{
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Today;

        public DateTime EndDate { get; set; } = DateTime.Today;

        [Ignore]
        public string DisplayTitle
        {
            get
            {
                DateTime today = DateTime.Today;

                bool isCurrent = today >= StartDate.Date && today <= EndDate.Date;

                return isCurrent
                    ? $"{Title} (Current)"
                    : Title;
            }
        }
    }
}
