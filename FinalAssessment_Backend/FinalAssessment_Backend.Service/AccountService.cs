using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using FinalAssessment_Backend.RepositoryInterface;
using FinalAssessment_Backend.ServiceInterface;
using FinalAssessment_Backend.Shared.EmailTemplates;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinalAssessment_Backend.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
       


        public AccountService(IAccountRepo accountRepo, IEmailService emailService, IJwtService jwtService) 
        {
            _accountRepo = accountRepo;
            _emailService = emailService;
            _jwtService = jwtService;
  
        }




        public async Task<(string msg, bool success, string? token)> loginUser(LoginCredentialsDto loginCredential)
        {
            var user = await _accountRepo.AuthenticateUser(loginCredential.Email, loginCredential.Password);

          

            if (user == null)
            {
                return ("wrong credential", false, null);
            }

            var tokenValue = await _jwtService.GenerateToken(user);


            if (user.IsActive == false)
            {

                MailRequest mailRequest = new MailRequest();
                mailRequest.ToEmail = user.Email;
                mailRequest.Subject = "Activate your account";
                mailRequest.Body = AccountEmailTemplate.GetTemplateActivateAccount(user, tokenValue);



                await _emailService.SendEmailAsync(mailRequest);
                return ("Not activated account, link send to your mail to activate account", false , null);
            }

            

            return ("User Logged In", true , tokenValue);

        }



        public async Task<bool> ActivateAccount(int userId)
        {
            var userToActivate = await _accountRepo.GetUserById(userId);

            if(userToActivate == null)
            {
                return false;
            }


            userToActivate.IsActive = true;

            return await _accountRepo.UpdateActivateAccount(userToActivate);
        }



        public async Task<(string msg, bool success)> forgotPasswordService(string email)
        {
            var user = await _accountRepo.GetUserByEmail(email);

            if(user == null)
            {
                return ("Wrong Credentials", false);
            }

            var tokenValue = await _jwtService.GenerateToken(user);

            var resetLink = $"http://localhost:4200/auth/reset-password/?token={tokenValue}";

            //Email Send to user
            MailRequest mailRequest = new MailRequest();
            mailRequest.ToEmail = user.Email;
            mailRequest.Subject = "Reset your password";
            mailRequest.Body = AccountEmailTemplate.GetTemplateResetPasswordAccount(user, resetLink);



            await _emailService.SendEmailAsync(mailRequest);

            return ("Email send to you for reset password", true);
        } 



        public async Task<(string msg, bool success)> resetPasswordService(ResetPasswordDto resetPasswordValue, string token)
        {
            //Extract token
            var userId = await _jwtService.ValidateJwtToken(token);

            if(userId == -1)
            {
                return ("Token expired", false);
            }

            var res = await _accountRepo.UpdatePassword(userId, resetPasswordValue.Password);

            return ("Password reset completed", true);

        }

       
    }
}
