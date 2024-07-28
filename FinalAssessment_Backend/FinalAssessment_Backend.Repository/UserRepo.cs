using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.RepositoryInterface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Repository
{
    public class UserRepo : IUserRepo
    {
        ApplicationDbContext _dbcontext;

        public UserRepo(ApplicationDbContext dbContext)
        {
            _dbcontext = dbContext;
        }


        public async Task<bool> InsertUser(PrashantDbUser user)
        {
            _dbcontext.PrashantDbUsers.Add(user);
            await _dbcontext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var affectedRows = await _dbcontext.Database.ExecuteSqlRawAsync(
            "EXEC SpDeleteUserPrashantDbUser @Id",
            new SqlParameter("@Id", id));

            await _dbcontext.SaveChangesAsync();

            // Return true if one or more rows were affected (deleted)
            return affectedRows > 0;
        }


        public async Task<(List<PrashantDbUser> Records, int TotalRecords)> GetRecords(int currentPage, int itemsPerPage)
        {
            var totalRecords = await _dbcontext.PrashantDbUsers
                              .CountAsync(u => u.IsDeleted == false);

            var users = await _dbcontext.PrashantDbUsers
                .Where(u => u.IsDeleted == false)
                .Include(u => u.PrashantDbAddresses) 
                .OrderBy(u => u.Id) 

                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)

                .ToListAsync();

            return (users, totalRecords);
        }
    }
}
