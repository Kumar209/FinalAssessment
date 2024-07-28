using FinalAssessment_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.RepositoryInterface
{
    public interface IAccountRepo
    {

        public Task<PrashantDbUser> GetUserById(int id);

        public  Task<bool> UpdateActivateAccount(PrashantDbUser user);

        public Task<PrashantDbUser> GetUserByEmail(string email);

        public Task<bool> UpdatePassword(int Id, string password);
    }
}
