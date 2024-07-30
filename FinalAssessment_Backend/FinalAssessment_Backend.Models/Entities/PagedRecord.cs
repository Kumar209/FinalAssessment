using FinalAssessment_Backend.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Models.Entities
{
    public class PagedRecord
    {
        public List<PrashantDbUser> Records { get; set; }

        public int TotalUsersCount { get; set; }

        public int TotalActiveCount { get; set; }

        public int TotalInactiveCount { get; set; }
    }
}
