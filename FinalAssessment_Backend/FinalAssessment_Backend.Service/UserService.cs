using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.RepositoryInterface;
using FinalAssessment_Backend.ServiceInterface;
using FinalAssessment_Backend.Shared.EmailTemplates;
using FinalAssessment_Backend.Shared.EncryptDecrypt;
using FinalAssessment_Backend.Shared.Hashing;
using OfficeOpenXml;
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
            var customPassword = await GenerateCustomPassword(8);


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

                /*  string template = GetTemplateUserCredential;
                  template = template.Replace("user.FirstName", userDetails.FirstName);
                  template = template.Replace("user.LastName", userDetails.LastName);

                  mailRequest.Body = template;*/



                await _emailService.SendEmailAsync(mailRequest);

            }

            return res;
        }


        public async Task<bool> RemoveUserById(int id)
        {
            return await _userRepo.DeleteUser(id);
        }




        public async Task<PageRecordDto> GetPagedRecords(UserQueryParams userQuery)
        {
            var res =  await _userRepo.GetRecords(userQuery);

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




        public async Task<byte[]> GenerateExcelAsync()
        {
            var users = await _userRepo.GetNonDeletedUsersAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("User Details");

                //Adding headers to excel
                worksheet.Cells[1, 1].Value = "First Name";
                worksheet.Cells[1, 2].Value = "Middle Name";
                worksheet.Cells[1, 3].Value = "Last Name";
                worksheet.Cells[1, 4].Value = "DOB";
                worksheet.Cells[1, 5].Value = "Date Of Joining";
                worksheet.Cells[1, 6].Value = "Email";
                worksheet.Cells[1, 7].Value = "Phone";
                worksheet.Cells[1, 8].Value = "Alternate Phone";
                worksheet.Cells[1, 9].Value = "Primary Address City";
                worksheet.Cells[1, 10].Value = "Primary Address State";
                worksheet.Cells[1, 11].Value = "Primary Address Country";
                worksheet.Cells[1, 12].Value = "Secondary Address City";
                worksheet.Cells[1, 13].Value = "Secondary Address State";
                worksheet.Cells[1, 14].Value = "Secondary Address Country";

             /*   worksheet.Row(1).Font.bold = true;*/



                int row = 2;

                foreach(var user in users)
                {
                    var primaryAddress = user.PrashantDbAddresses.FirstOrDefault(a => a.AddressTypeId == 1); 
                    var secondaryAddress = user.PrashantDbAddresses.FirstOrDefault(a => a.AddressTypeId == 2);


                    worksheet.Cells[row, 1].Value = user.FirstName;


                    worksheet.Cells[row, 2].Value = user.MiddleName;

                    worksheet.Cells[row, 3].Value = user.LastName;


                    worksheet.Cells[row, 4].Value = user.DateOfBirth;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "yyyy-mm-dd";


                    worksheet.Cells[row, 5].Value = user.DateOfJoining;
                    worksheet.Cells[row, 5].Style.Numberformat.Format = "yyyy-mm-dd";


                    worksheet.Cells[row, 6].Value = _encryptDecrypt.DecryptCipherText(user.Email);


                    worksheet.Cells[row, 7].Value = _encryptDecrypt.DecryptCipherText(user.Phone);


                    worksheet.Cells[row, 8].Value = user.AlternatePhone != null
                                                    ? _encryptDecrypt.DecryptCipherText(user.AlternatePhone)
                                                    : null;


                    worksheet.Cells[row, 9].Value = primaryAddress.City;


                    worksheet.Cells[row, 10].Value = primaryAddress.State;


                    worksheet.Cells[row, 11].Value = primaryAddress.Country;


                    worksheet.Cells[row, 12].Value = secondaryAddress?.City;


                    worksheet.Cells[row, 13].Value = secondaryAddress?.State;


                    worksheet.Cells[row, 14].Value = secondaryAddress?.Country;


                    row++;
                }

                worksheet.Cells.AutoFitColumns();


                return package.GetAsByteArray();
            }
        }





        public async Task<PrashantDbUserDto> GetUserDetailById(int id)
        {
            var res = await _userRepo.GetUserById(id);

            var responseDto = new PrashantDbUserDto
            {
                Id = res.Id,
                FirstName = res.FirstName,
                MiddleName = res.MiddleName,
                LastName = res.LastName,

                Email = _encryptDecrypt.DecryptCipherText(res.Email),

                Gender = res.Gender,
                DateOfJoining = res.DateOfJoining,
                DateOfBirth = res.DateOfBirth,

                Phone = _encryptDecrypt.DecryptCipherText(res.Phone),

                AlternatePhone = (res.AlternatePhone != null && res.AlternatePhone.Length > 0)
                                 ? _encryptDecrypt.DecryptCipherText(res.AlternatePhone)
                                 : null,

                ImageUrl = res.ImageUrl,
                PrashantDbAddresses = res.PrashantDbAddresses.Select(address => new PrashantDbAddressDto
                {
                    Id = address.Id,
                    City = address.City,
                    State = address.State,
                    Country = address.Country,
                    ZipCode = address.ZipCode,
                    AddressTypeId = address.AddressTypeId,
                    UserId = address.UserId
                }).ToList(),
            };


            return responseDto;
        }



        public async Task<bool> UpdateUserDetails(PrashantDbUserDto userDetails)
        {
            var userId = userDetails.Id ?? 0;

            var userFromDb = await _userRepo.GetUserById(userId);

            var userDetailsEntity = new PrashantDbUser
            {
                FirstName = userDetails.FirstName,
                MiddleName = userDetails.MiddleName,
                LastName = userDetails.LastName,


                //We will not update the email due to token
                Email = userFromDb.Email,

                Gender = userDetails.Gender,
                DateOfJoining = userDetails.DateOfJoining,
                DateOfBirth = userDetails.DateOfBirth,

                Phone = _encryptDecrypt.EncryptPlainText(userDetails.Phone),
                AlternatePhone = !string.IsNullOrEmpty(userDetails.AlternatePhone) ? _encryptDecrypt.EncryptPlainText(userDetails.AlternatePhone) : null,

                ImageUrl = await _imageUploadService.GetImageUrl(userDetails.ImageFile),

                Password = userFromDb.Password,

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

            var res = await _userRepo.UpdateUser(userDetailsEntity);

         

            return res;
        }


    }
}
