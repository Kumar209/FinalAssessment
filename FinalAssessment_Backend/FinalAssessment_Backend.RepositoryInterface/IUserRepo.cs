using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.RepositoryInterface
{
    public interface IUserRepo
    {
        public Task<bool> InsertUser(PrashantDbUser user);

        public Task<bool> DeleteUser(int id);

        public Task<(List<PrashantDbUser> Records, int TotalRecords)> GetRecords(int currentPage, int itemsPerPage);
    }
}
