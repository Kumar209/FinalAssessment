using FinalAssessment_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Shared.EmailTemplates
{
    public static  class AccountEmailTemplate
    {
        public static  string GetTemplateActivateAccount(PrashantDbUser user, string token)
        {
            return $@"<html>
               <head></head>      
              <body style="" margin:0;padding:0;font-family:Arial,Helvetica,sans-serif;"">
               <div style=""height:auto;background: linear-gradient(to top, #c9c9ff 50%,#6e6ef6 90%) no-repeat;width:400px;padding:30px;"">
                   <div>
                          <div>
                             <h1>Dear ""{user.FirstName}""</h2>
                             <h1>Activate your account</h1>
                              <hr>
                              <p>You're receiving this mail because your account is not active</p>
                               <p> Please tap the button below to choose a new password.</p>
                               <a href=""http;//localhost:4200/reset?email={user.Email}&code={token}"" target=""_blank"" style=""background:#0d6efc;
                                     color:white;border-radius:4px ;display:block;margin:0 auto;width:50%;text-align:ceneter;text-decoration:none"">Activate Account</a>
                               <p>Kind Regards,<br><br>
                                  Prashant and groups</p>
                          </div>
                   </div>
               </div>
             </body>
             </html>";
        }

        public static string GetTemplateResetPasswordAccount(PrashantDbUser user, string resetLink)
        {
            return $@"<html>
               <head></head>      
              <body style="" margin:0;padding:0;font-family:Arial,Helvetica,sans-serif;"">
               <div style=""height:auto;background: linear-gradient(to top, #c9c9ff 50%,#6e6ef6 90%) no-repeat;width:400px;padding:30px;"">
                   <div>
                          <div>
                             <h1>Dear ""{user.FirstName} {user.MiddleName} ""</h2>
                             <h1>Reset your password</h1>
                              <hr>
                              <p>You're receiving this mail because you want to reset your password</p>
                               <p> Please tap the button below to choose a new password.</p>
                               <a href=""{resetLink}"" target=""_blank"" style=""background:#0d6efc;
                                     color:white;border-radius:4px ;display:block;margin:0 auto;width:50%;text-align:ceneter;text-decoration:none"">Reset Password</a>
                               <p>Kind Regards,<br><br>
                                  Prashant and groups</p>
                          </div>
                   </div>
               </div>
             </body>
             </html>";
        }
    }
}
