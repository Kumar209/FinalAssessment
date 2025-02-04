﻿using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.ServiceInterface
{
    public interface IAccountService
    {

        public Task<(string msg, bool success, string? token, RequiredDataFrontend? requiredDataForFrontend)> loginUser(LoginCredentialsDto loginCredential);

        public  Task<(string msg, bool success)> ActivateAccount(string token);

        public Task<(string msg, bool success)> forgotPasswordService(string email);

        public Task<(string msg, bool success)> resetPasswordService(ResetPasswordDto resetPasswordValue, string token);

        public Task<(string msg, bool success)> changePasswordService(ChangePasswordDto changePasswordValue, string token);
    }
}
