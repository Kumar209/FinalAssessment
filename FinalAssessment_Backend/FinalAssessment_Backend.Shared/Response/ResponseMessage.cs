using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Shared.Response
{
    public static class ResponseMessage
    {
        public const string validationFailed = "Validation failed";
        public const string wrongEmail = "Wrong Email";
        public const string wrongPassword = "Wrong Password";
        public const string loginSuccess = "Login successfully";
        public const string wrongCredential = "Wrong Credentials";
        public const string inactiveaccountmsg = "InActive";
        public const string addedUserSuccess = "Successfully created user and credential send to email";
        public const string deleteUserSuccess = "User is deleted";
        public const string invalidUser = "Your are not a registered user";
        public const string updateUserSuccess = "Successfully updated user";
        public const string oldPasswordIncorrect = "Your old password is incorrect";
        public const string tokenError = "Your token is expired or invalid token";
        public const string unauthorizeUser = "Unauthorized User";
        public const string getdetailMessage = "Success";
        public const string internalServerError = "Internal server error";

        public const string activatedAccountMsg = "Activated Account";
        public const string activationAccountError = "Error Activating Account";

        public const string emailSentReset = "Email send to you for reset password";

        public const string passwordResetSuccess = "Password reset completed";
        public const string passwordChangeSuccess = "Password changed";



        public const string somethingWrongError = "Something went wrong";
    }
}
