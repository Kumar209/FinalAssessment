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
        public Task<(bool success, PrashantDbUser user)> InsertUser(PrashantDbUser user);

        public Task<bool> DeleteUser(int id);

        public Task<PagedRecord> GetRecords(UserQueryParams userQuery);

        public Task<List<PrashantDbUser>> GetNonDeletedUsersAsync();

        public Task<PrashantDbUser> GetUserById(int id);

        public Task<bool> UpdateUser(PrashantDbUser user);
    }
}
