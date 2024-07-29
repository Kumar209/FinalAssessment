using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.RepositoryInterface;
using FinalAssessment_Backend.ServiceInterface;
using FinalAssessment_Backend.Shared.EmailTemplates;
using FinalAssessment_Backend.Shared.EncryptDecrypt;
using FinalAssessment_Backend.Shared.Hashing;


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




        public async Task<(string msg, bool success, string? token)> loginUser(LoginCredentialsDto loginCredential)
        {
            var user = await _accountRepo.GetUserByEmail(loginCredential.Email);

            if (user == null)
            {
                return ("Wrong email", false, null);
            }

            //Verifying the password due to it is hashed in db
            var validatePassword = _hashing.VerifyHash(loginCredential.Password, user.Password);

            if (validatePassword == false)
            {
                return ("Wrong password", false, null);
            }

            var tokenValue = await _jwtService.GenerateToken(user);


            if (user.IsActive == false)
            {

                MailRequest mailRequest = new MailRequest();
                mailRequest.ToEmail = _encyptDecrypt.DecryptCipherText(user.Email);
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
