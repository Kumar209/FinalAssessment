using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.RepositoryInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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



        public async Task<PagedRecord> GetRecords(UserQueryParams userQuery)
        {

            var query = _dbcontext.PrashantDbUsers
                        .Include(u => u.PrashantDbAddresses)
                        .Where(u => u.IsDeleted == false);

            // Giving total counts of users which are present in db means including active + inactive
            var totalUsersCount = await _dbcontext.PrashantDbUsers.CountAsync(u => u.IsDeleted == false);


            //Giving total Active user Counts
            var totalActiveCount = await _dbcontext.PrashantDbUsers.CountAsync(u => u.IsDeleted == false && u.IsActive == true);


            // Giving total inactive user counts
            var totalInactiveCount = await _dbcontext.PrashantDbUsers.CountAsync(u => u.IsDeleted == false && u.IsActive == false);


            // Applying filteration like deciding query to apply if status is not null : for active and inactive filtration
            if (!string.IsNullOrEmpty(userQuery.Status))
            {
                if (userQuery.Status.Equals("active", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(u => u.IsActive == true);
                }
                else if (userQuery.Status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(u => u.IsActive == false);
                }
            }


            //Here SortBy is a column name
            if (!string.IsNullOrEmpty(userQuery.SortBy))
            {
                //Handles the dynamic column name on that basis to be sort used :EF.Property<object> other wise we have to use switch case
                query = userQuery.IsAscending ? query.OrderBy(e => EF.Property<object>(e, userQuery.SortBy)) : query.OrderByDescending(e => EF.Property<object>(e, userQuery.SortBy));
            }
            else
            {
                query = query.OrderBy(u => u.Id);
            }


            var users = await query
                        .Skip((userQuery.CurrentPage - 1) * userQuery.ItemsPerPage)
                        .Take(userQuery.ItemsPerPage)
                        .ToListAsync();



            return new PagedRecord
            {
                TotalUsersCount = totalUsersCount,
                TotalActiveCount = totalActiveCount,
                TotalInactiveCount = totalInactiveCount,
                Records = users
            };
        }





        //API for excel file
        public async Task<List<PrashantDbUser>> GetNonDeletedUsersAsync()
        {
            return await _dbcontext.PrashantDbUsers
                                 .Where(u => u.IsDeleted == false)
                                 .Include(u => u.PrashantDbAddresses)
                                 .ToListAsync();
        }



        public async Task<PrashantDbUser> GetUserById(int id)
        {
            return await _dbcontext.PrashantDbUsers
                                   .Include(u => u.PrashantDbAddresses)
                                   .FirstOrDefaultAsync(u => u.Id == id);
        }


    }
}
