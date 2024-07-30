using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.RepositoryInterface;
using FinalAssessment_Backend.ServiceInterface;
using FinalAssessment_Backend.Shared.EmailTemplates;
using FinalAssessment_Backend.Shared.EncryptDecrypt;
using FinalAssessment_Backend.Shared.Hashing;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IImageUploadService _imageUploadService;
        private readonly IHashing _hashing;
        private readonly EncryptDecrypt _encryptDecrypt;
        private readonly IEmailService _emailService;

        public UserService(IUserRepo userRepo , IImageUploadService imageUploadService, IEmailService emailService , IHashing hashing, EncryptDecrypt encryptDecrypt)
        {
            _userRepo = userRepo;
            _imageUploadService = imageUploadService;
            _hashing = hashing;
            _encryptDecrypt = encryptDecrypt;
            _emailService = emailService;
        }


        public async Task<string> GenerateCustomPassword(int len)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";

            var password = new char[len];

            for (int i = 0; i < len; i++)
            {
                password[i] = validChars[new Random().Next(validChars.Length)];
            }

            return new string(password);
        }


       

        public async Task<bool> CreateUser(PrashantDbUserDto userDetails)
        {
            var customPassword = "Pass@123";


            var userDetailsEntity = new PrashantDbUser
            {
                FirstName = userDetails.FirstName,
                MiddleName = userDetails.MiddleName,
                LastName = userDetails.LastName,

                Email =  _encryptDecrypt.EncryptPlainText(userDetails.Email),

                Gender = userDetails.Gender,
                DateOfJoining = userDetails.DateOfJoining,
                DateOfBirth = userDetails.DateOfBirth,

                Phone = _encryptDecrypt.EncryptPlainText(userDetails.Phone),
                AlternatePhone = !string.IsNullOrEmpty(userDetails.AlternatePhone) ? _encryptDecrypt.EncryptPlainText(userDetails.AlternatePhone) : null,

                ImageUrl = await _imageUploadService.GetImageUrl(userDetails.ImageFile),

                Password = _hashing.GenerateHash(customPassword),

                CreatedBy = userDetails.FirstName + userDetails.MiddleName,
                PrashantDbAddresses = userDetails.PrashantDbAddresses.Select(a => new PrashantDbAddress
                {
                    City = a.City,
                    State = a.State,
                    Country = a.Country,
                    ZipCode = a.ZipCode,
                    AddressTypeId = a.AddressTypeId
                }).ToList()
            };

            var res = await _userRepo.InsertUser(userDetailsEntity);


            //Email sending for credential
            if(res == true)
            {
                MailRequest mailRequest = new MailRequest();
                mailRequest.ToEmail = _encryptDecrypt.DecryptCipherText(userDetailsEntity.Email);
                mailRequest.Subject = "User Credentail by Kumar Enterprise";
                mailRequest.Body = UserEmailTemplate.GetTemplateUserCredential(userDetails, customPassword);



                await _emailService.SendEmailAsync(mailRequest);

            }

            return res;
        }


        public async Task<bool> RemoveUserById(int id)
        {
            return await _userRepo.DeleteUser(id);
        }




        public async Task<PageRecordDto> GetPagedRecords(int currentPage, int itemsPerPage, string status)
        {
            var res =  await _userRepo.GetRecords(currentPage, itemsPerPage, status);

            var mappedData = res.Records.Select(user => new PrashantDbUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,

                Email = _encryptDecrypt.DecryptCipherText(user.Email),

                Gender = user.Gender,
                DateOfJoining = user.DateOfJoining,
                DateOfBirth = user.DateOfBirth,

                Phone = _encryptDecrypt.DecryptCipherText(user.Phone),
                AlternatePhone = (user.AlternatePhone != null && user.AlternatePhone.Length > 0)
                                 ? _encryptDecrypt.DecryptCipherText(user.AlternatePhone)
                                 : null,

                ImageUrl = user.ImageUrl,
                IsActive = user.IsActive,
                PrashantDbAddresses = user.PrashantDbAddresses.Select(address => new PrashantDbAddressDto
                {
                    Id = address.Id,
                    City = address.City,
                    State = address.State,
                    Country = address.Country,
                    ZipCode = address.ZipCode,
                    AddressTypeId = address.AddressTypeId,
                    UserId = address.UserId
                }).ToList()
            }).ToList();


            return new PageRecordDto
            {
                TotalUsersCount = res.TotalUsersCount,
                TotalActiveCount = res.TotalActiveCount,
                TotalInactiveCount = res.TotalInactiveCount,
                Records = mappedData
            };
        }



    }
}
