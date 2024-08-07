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



        public async Task<(bool success, PrashantDbUser user)> InsertUser(PrashantDbUser user)
        {
            _dbcontext.PrashantDbUsers.Add(user);
            await _dbcontext.SaveChangesAsync();
            return (true, user);
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

                    default:
                        query = query.OrderBy(u => u.Id); 
                        break;
                }
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
            userFromDb.ImageUrl = user.ImageUrl;

            // List of address IDs that are to be kept
            var addressesToKeep = new HashSet<int>(user.PrashantDbAddresses.Select(a => a.AddressTypeId));

            // Iterate through existing addresses to update or remove
            foreach (var address in userFromDb.PrashantDbAddresses.ToList())
            {
                if (addressesToKeep.Contains(address.AddressTypeId))
                {
                    // Update existing address
                    var updatedAddress = user.PrashantDbAddresses.First(a => a.AddressTypeId == address.AddressTypeId);
                    address.City = updatedAddress.City;
                    address.State = updatedAddress.State;
                    address.Country = updatedAddress.Country;
                    address.ZipCode = updatedAddress.ZipCode;
                }
                else
                {
                    // Remove address that is not in the updated list
                    _dbcontext.PrashantDbAddresses.Remove(address);
                }
            }

            // Add new addresses that are in the update but not in the existing list
            foreach (var addressDto in user.PrashantDbAddresses)
            {
                var existingAddress = userFromDb.PrashantDbAddresses.FirstOrDefault(a => a.AddressTypeId == addressDto.AddressTypeId);

                if (existingAddress == null)
                {
                    // Add new address
                    userFromDb.PrashantDbAddresses.Add(new PrashantDbAddress
                    {
                        AddressTypeId = addressDto.AddressTypeId,
                        City = addressDto.City,
                        State = addressDto.State,
                        Country = addressDto.Country,
                        ZipCode = addressDto.ZipCode,
                    });
                }
            }

            await _dbcontext.SaveChangesAsync();
            return true;
        }


    }
}
