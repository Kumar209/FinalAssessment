using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.RepositoryInterface;
using FinalAssessment_Backend.ServiceInterface;
using FinalAssessment_Backend.Shared.EmailTemplates;
using FinalAssessment_Backend.Shared.EncryptDecrypt;
using FinalAssessment_Backend.Shared.Hashing;
using FinalAssessment_Backend.Shared.Response;
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
                return (ResponseMessage.wrongEmail, false, null, null);
            }

            //Verifying the password due to it is hashed in db
            var validatePassword = _hashing.VerifyHash(loginCredential.Password, user.Password);



            if (validatePassword == false)
            {
                return (ResponseMessage.wrongPassword, false, null, null);
            }

            //For localStorage in frontend
            var requiredDataForFrontend = new RequiredDataFrontend
            {
                UserName = user.FirstName + " " + user.MiddleName + (string.IsNullOrEmpty(user.LastName) ? "" : " " + user.LastName),
                ImageUrl = user.ImageUrl
            };

            var tokenValue = await _jwtService.GenerateToken(user);


            if (user.IsActive == false)
            {
                var activationLink = $"http://localhost:4200/auth/activate-account/?token={tokenValue}";

                MailRequest mailRequest = new MailRequest();
                mailRequest.ToEmail = _encyptDecrypt.DecryptCipherText(user.Email);
                mailRequest.Subject = "Activate your account";
                mailRequest.Body = AccountEmailTemplate.GetTemplateActivateAccount(user, activationLink);


                await _emailService.SendEmailAsync(mailRequest);
                return (ResponseMessage.inactiveaccountmsg, false, null, null); 
            }

            

            return (ResponseMessage.loginSuccess, true , tokenValue, requiredDataForFrontend);

        }



        public async Task<(string msg, bool success)> ActivateAccount(string token)
        {
            var userId = await _jwtService.ValidateJwtToken(token);

            if (userId == -1)
            {
                return (ResponseMessage.tokenError, false);
            }

            var userToActivate = await _accountRepo.GetUserById(userId);

            userToActivate.IsActive = true;

            var response = await _accountRepo.UpdateActivateAccount(userToActivate);

            if (response)
            {
                return (ResponseMessage.activatedAccountMsg, true);
            }

            return (ResponseMessage.activationAccountError, false);

            
        }



        public async Task<(string msg, bool success)> forgotPasswordService(string email)
        {
            var user = await _accountRepo.GetUserByEmail(email);

            if(user == null)
            {
                return (ResponseMessage.wrongCredential, false);
            }

            var tokenValue = await _jwtService.GenerateToken(user);

            var resetLink = $"http://localhost:4200/auth/reset-password/?token={tokenValue}";

            //Email Send to user
            MailRequest mailRequest = new MailRequest();
            mailRequest.ToEmail = _encyptDecrypt.DecryptCipherText(user.Email);
            mailRequest.Subject = "Reset your password";
            mailRequest.Body = AccountEmailTemplate.GetTemplateResetPasswordAccount(user, resetLink);



            await _emailService.SendEmailAsync(mailRequest);

            return (ResponseMessage.emailSentReset, true);
        } 



        public async Task<(string msg, bool success)> resetPasswordService(ResetPasswordDto resetPasswordValue, string token)
        {
            var userId = await _jwtService.ValidateJwtToken(token);

            if(userId == -1)
            {
                return (ResponseMessage.tokenError, false);
            }

            var res = await _accountRepo.UpdatePassword(userId, resetPasswordValue.Password);

            return (ResponseMessage.passwordResetSuccess, true);

        }


        public async Task<(string msg ,bool success)> changePasswordService(ChangePasswordDto changePasswordValue, string token)
        {
            var userId = await _jwtService.ValidateJwtToken(token);

            if(userId == -1)
            {
                return (ResponseMessage.tokenError, false);
            }

            var user = await _accountRepo.GetUserById(userId);

            var validateOldPassword = _hashing.VerifyHash(changePasswordValue.OldPassword, user.Password);

            if(validateOldPassword == false)
            {
                return (ResponseMessage.oldPasswordIncorrect, false);
            }

            var res = await _accountRepo.UpdatePassword(userId, changePasswordValue.NewPassword);

            return (ResponseMessage.passwordChangeSuccess, true);
        }

       
    }
}
