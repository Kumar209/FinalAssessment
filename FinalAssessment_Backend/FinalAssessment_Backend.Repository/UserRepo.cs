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
            //Not working for city and state cant able to navigate
            /*  if (!string.IsNullOrEmpty(userQuery.SortBy))
              {
                  //Handles the dynamic column name on that basis to be sort used :EF.Property<object> other wise we have to use switch case
                  query = userQuery.IsAscending ? query.OrderBy(e => EF.Property<object>(e, userQuery.SortBy)) : query.OrderByDescending(e => EF.Property<object>(e, userQuery.SortBy));
              }
              else
              {
                  query = query.OrderBy(u => u.Id);
              }*/



            //Sorting using switch case
            if (!string.IsNullOrEmpty(userQuery.SortBy))
            {
                switch (userQuery.SortBy.ToLower())
                {
                    case "firstname":
                        query = userQuery.IsAscending ? query.OrderBy(u => u.FirstName) : query.OrderByDescending(u => u.FirstName);
                        break;
                    case "middlename":
                        query = userQuery.IsAscending ? query.OrderBy(u => u.MiddleName) : query.OrderByDescending(u => u.MiddleName);
                        break;
                    case "lastname":
                        query = userQuery.IsAscending ? query.OrderBy(u => u.LastName) : query.OrderByDescending(u => u.LastName);
                        break;
                    case "email":
                        query = userQuery.IsAscending ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email);
                        break;
                    case "phone":
                        query = userQuery.IsAscending ? query.OrderBy(u => u.Phone) : query.OrderByDescending(u => u.Phone);
                        break;
                    case "dateofbirth":
                        query = userQuery.IsAscending ? query.OrderBy(u => u.DateOfBirth) : query.OrderByDescending(u => u.DateOfBirth);
                        break;
                    case "city":
                        query = userQuery.IsAscending ? query.OrderBy(u => u.PrashantDbAddresses.FirstOrDefault().City) : query.OrderByDescending(u => u.PrashantDbAddresses.FirstOrDefault().City);
                        break;
                    case "state":
                        query = userQuery.IsAscending ? query.OrderBy(u => u.PrashantDbAddresses.FirstOrDefault().State) : query.OrderByDescending(u => u.PrashantDbAddresses.FirstOrDefault().State);
                        break;
                    // Add more cases for other properties as needed
                    default:
                        query = query.OrderBy(u => u.Id); // Default sort by Id
                        break;
                }
            }
            else
            {
                query = query.OrderBy(u => u.Id); // Default sort by Id
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



        public async Task<bool> UpdateUser(PrashantDbUser user)
        {
            var userId = user.Id;

            var userFromDb = await GetUserById(userId);

            if (userFromDb == null)
            {
                return false; 
            }


            userFromDb.FirstName = user.FirstName;
            userFromDb.MiddleName = user.MiddleName;
            userFromDb.LastName = user.LastName;
            userFromDb.Phone = user.Phone;
            userFromDb.AlternatePhone = user.AlternatePhone;
            userFromDb.DateOfJoining = user.DateOfJoining;
            userFromDb.DateOfBirth = user.DateOfBirth;
            userFromDb.Gender = user.Gender;
           /* userFromDb.Email = EncriptionAndDecription.EncryptData(userDetailsAnkitDto.Email);*/
          /*  userFromDb.IsActive = user.IsActive;*/

            // Update or add user addresses
            foreach (var addressDto in user.PrashantDbAddresses)
            {
                var address = userFromDb.PrashantDbAddresses.FirstOrDefault(a => a.AddressTypeId == addressDto.AddressTypeId);

                if (address != null)
                {
                    // Update existing address
                    address.City = addressDto.City;
                    address.State = addressDto.State;
                    address.Country = addressDto.Country;
                    address.ZipCode = addressDto.ZipCode;
                }
                else
                {
                    // Add new address
                    user.PrashantDbAddresses.Add(new PrashantDbAddress
                    {
                        City = addressDto.City,
                        State = addressDto.State,
                        Country = addressDto.Country,
                        ZipCode = addressDto.ZipCode,
                        UserId = user.Id
                    });
                }
            }

            await _dbcontext.SaveChangesAsync();
            return true;

            /*_dbcontext.PrashantDbUsers.Update(user);
            await _dbcontext.SaveChangesAsync();
            return true;*/

            /* _dbcontext.Entry(user).State = EntityState.Modified;
             await _dbcontext.SaveChangesAsync();
             return true;*/
        }


    }
}
