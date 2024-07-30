using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Models.Dto
{
    public class UserQueryParams
    {
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; } 
        public string? Status { get; set; }
        public string? SortBy { get; set; }
        public bool IsAscending { get; set; } = true;
    }
}
