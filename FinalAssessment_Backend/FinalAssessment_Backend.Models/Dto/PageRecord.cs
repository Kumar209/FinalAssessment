using FinalAssessment_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Models.Dto
{
    public class PageRecord
    {
        public List<PrashantDbUser> Records {  get; set; }
        public int TotalRecords { get; set; }
    }
}
