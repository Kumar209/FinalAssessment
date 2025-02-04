﻿using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.RepositoryInterface;
using FinalAssessment_Backend.Shared.EncryptDecrypt;
using FinalAssessment_Backend.Shared.Hashing;
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
        private readonly EncryptDecrypt _encryptDecrypt;
        private readonly IHashing _hashing;

        public AccountRepo(ApplicationDbContext dbContext, EncryptDecrypt encryptDecrypt , IHashing hashing)
        {
            _dbContext = dbContext;
            _encryptDecrypt = encryptDecrypt;
            _hashing = hashing;
        }


        public async Task<PrashantDbUser> GetUserById(int id)
        {
            return await _dbContext.PrashantDbUsers.FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == false);
        }


        public async Task<bool> UpdateActivateAccount(PrashantDbUser user)
        {

            _dbContext.PrashantDbUsers.Update(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<PrashantDbUser> GetUserByEmail(string email)
        {


            var allUsers = await _dbContext.PrashantDbUsers
                           .Include(u => u.PrashantDbAddresses)
                           .ToListAsync();

            var user = allUsers.FirstOrDefault(o => _encryptDecrypt.DecryptCipherText(o.Email) == email && o.IsDeleted == false);


            return user;
        }


        public async Task<bool> UpdatePassword(int Id, string password)
        {
            var hashedPassword = _hashing.GenerateHash(password);

            var affectedRows = await _dbContext.Database
                              .ExecuteSqlRawAsync(
                               "SpUpdatePasswordPrashantDbUser @Id, @Password",
                               new SqlParameter("@Id", Id),
                               new SqlParameter("@Password", hashedPassword)
                              );


            return affectedRows > 0;
        }


    }
}
