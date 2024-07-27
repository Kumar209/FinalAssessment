using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.RepositoryInterface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Repository
{
    public class AccountRepo : IAccountRepo
    {
        ApplicationDbContext _dbContext;

        public AccountRepo(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<PrashantDbUser> AuthenticateUser(string email, string password)
        {
            var user = await _dbContext.PrashantDbUsers.FirstOrDefaultAsync(u => u.Email == email && u.Password == password );

            return user;
        }


        public async Task<PrashantDbUser> GetUserById(int id)
        {
            return await _dbContext.PrashantDbUsers.FirstOrDefaultAsync(u => u.Id == id);
        }


        public async Task<bool> UpdateActivateAccount(PrashantDbUser user)
        {
            _dbContext.PrashantDbUsers.Update(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<PrashantDbUser> GetUserByEmail(string email)
        {

            var user = await _dbContext.PrashantDbUsers
                       .Include(o => o.PrashantDbAddresses)
                       .FirstOrDefaultAsync(o => o.Email == email );


            return user;
        }


        public async Task<bool> UpdatePassword(int Id, string password)
        {
            var affectedRows = await _dbContext.Database
                              .ExecuteSqlRawAsync(
                               "SpUpdatePasswordUsersPrashantDb @Id, @Password",
                               new SqlParameter("@Id", Id),
                               new SqlParameter("@Password", password)
                              );

            await _dbContext.SaveChangesAsync();

            return affectedRows > 0;
        }


    }
}
