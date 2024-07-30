using FinalAssessment_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Models.Dto
{
    public class PageRecordDto
    {
        public List<PrashantDbUserDto> Records {  get; set; }

        public int TotalUsersCount { get; set; }

        public int TotalActiveCount { get; set; }

        public int TotalInactiveCount { get; set;}
    }
}
