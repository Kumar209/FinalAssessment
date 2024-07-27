using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.RepositoryInterface;
using FinalAssessment_Backend.ServiceInterface;
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
        public UserService(IUserRepo userRepo , IImageUploadService imageUploadService)
        {
            _userRepo = userRepo;
            _imageUploadService = imageUploadService;
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


            var userDetailsEntity = new PrashantDbUser
            {
                FirstName = userDetails.FirstName,
                MiddleName = userDetails.MiddleName,
                LastName = userDetails.LastName,
                Email = userDetails.Email,
                Gender = userDetails.Gender,
                DateOfJoining = userDetails.DateOfJoining,
                DateOfBirth = userDetails.DateOfBirth,
                Phone = userDetails.Phone,
                AlternatePhone = userDetails.AlternatePhone,
                ImageUrl = await _imageUploadService.GetImageUrl(userDetails.ImageFile),
                Password = "Pass@123",
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

            return await _userRepo.InsertUser(userDetailsEntity);
        }


        public async Task<bool> RemoveUserById(int id)
        {
            return await _userRepo.DeleteUser(id);
        }
    }
}
