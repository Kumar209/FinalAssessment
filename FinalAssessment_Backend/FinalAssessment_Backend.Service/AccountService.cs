using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.RepositoryInterface;
using FinalAssessment_Backend.ServiceInterface;
using FinalAssessment_Backend.Shared.EmailTemplates;
using FinalAssessment_Backend.Shared.EncryptDecrypt;
using FinalAssessment_Backend.Shared.Hashing;
using Org.BouncyCastle.Asn1.Cms;


namespace FinalAssessment_Backend.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _accountRepo;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly EncryptDecrypt _encyptDecrypt;
        private readonly IHashing _hashing;
       


        public AccountService(IAccountRepo accountRepo, IEmailService emailService, IJwtService jwtService, EncryptDecrypt encryptDecrypt, IHashing hashing) 
        {
            _accountRepo = accountRepo;
            _emailService = emailService;
            _jwtService = jwtService;
            _encyptDecrypt = encryptDecrypt;
            _hashing = hashing;
        }




        public async Task<(string msg, bool success, string? token, RequiredDataFrontend? requiredDataForFrontend)> loginUser(LoginCredentialsDto loginCredential)
        {
            var user = await _accountRepo.GetUserByEmail(loginCredential.Email);

            if (user == null)
            {
                return ("Wrong email", false, null, null);
            }

            //Verifying the password due to it is hashed in db
            var validatePassword = _hashing.VerifyHash(loginCredential.Password, user.Password);



            if (validatePassword == false)
            {
                return ("Wrong password", false, null, null);
            }

            //For localStorage in frontend
            var requiredDataForFrontend = new RequiredDataFrontend
            {
                UserName = user.FirstName + " " + user.MiddleName + (string.IsNullOrEmpty(user.LastName) ? "" : " " + user.LastName),
                ImageUrl = user.ImageUrl
            };

            var tokenValue = await _jwtService.GenerateToken(user);

            var activationLink = $"http://localhost:4200/auth/activate-account/?token={tokenValue}";


            if (user.IsActive == false)
            {
                MailRequest mailRequest = new MailRequest();
                mailRequest.ToEmail = _encyptDecrypt.DecryptCipherText(user.Email);
                mailRequest.Subject = "Activate your account";
                mailRequest.Body = AccountEmailTemplate.GetTemplateActivateAccount(user, activationLink);


                await _emailService.SendEmailAsync(mailRequest);
                return ("InActive", false, null, null); 
            }

            

            return ("User Logged In", true , tokenValue, requiredDataForFrontend);

        }



        public async Task<(string msg, bool success)> ActivateAccount(string token)
        {
            var userId = await _jwtService.ValidateJwtToken(token);

            if (userId == -1)
            {
                return ("Token expired", false);
            }

            var userToActivate = await _accountRepo.GetUserById(userId);

            userToActivate.IsActive = true;

            var response = await _accountRepo.UpdateActivateAccount(userToActivate);

            if (response)
            {
                return ("Activated Account", true);
            }

            return ("Error Activating Account", false);

            
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
            mailRequest.ToEmail = _encyptDecrypt.DecryptCipherText(user.Email);
            mailRequest.Subject = "Reset your password";
            mailRequest.Body = AccountEmailTemplate.GetTemplateResetPasswordAccount(user, resetLink);



            await _emailService.SendEmailAsync(mailRequest);

            return ("Email send to you for reset password", true);
        } 



        public async Task<(string msg, bool success)> resetPasswordService(ResetPasswordDto resetPasswordValue, string token)
        {
            var userId = await _jwtService.ValidateJwtToken(token);

            if(userId == -1)
            {
                return ("Token expired", false);
            }

            var res = await _accountRepo.UpdatePassword(userId, resetPasswordValue.Password);

            return ("Password reset completed", true);

        }


        public async Task<(string msg ,bool success)> changePasswordService(ChangePasswordDto changePasswordValue, string token)
        {
            var userId = await _jwtService.ValidateJwtToken(token);

            if(userId == -1)
            {
                return ("Token expired", false);
            }

            var user = await _accountRepo.GetUserById(userId);

            var validateOldPassword = _hashing.VerifyHash(changePasswordValue.OldPassword, user.Password);

            if(validateOldPassword == false)
            {
                return ("Wrong old password", false);
            }

            var res = await _accountRepo.UpdatePassword(userId, changePasswordValue.NewPassword);

            return ("Password changed", true);
        }

       
    }
}
