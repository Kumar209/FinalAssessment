﻿using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Shared.EmailTemplates
{
    public static class UserEmailTemplate
    {
        public static string GetTemplateUserCredential(PrashantDbUserDto user, string password, string activationLink)
        {
            return $@"<html>
               <head></head>      
              <body style="" margin:0;padding:0;font-family:Arial,Helvetica,sans-serif;"">
               <div style=""height:auto;background: linear-gradient(to top, #c9c9ff 50%,#6e6ef6 90%) no-repeat;width:600px;padding:30px;"">
                   <div>
                          <div>
                             <h1>Dear ""{user.FirstName} {user.MiddleName} ""</h2>
                             <h1>User Credential</h1>
                              <hr>
                              <p>You're receiving this mail for Credentail generated</p>

                             
                               <p>Email : ""{user.Email}"" </p>
                               <p>Password : ""{password}"" </p>


                               <a href=""{activationLink}"" target=""_blank"" style=""background:#0d6efc;
                                color:white;border-radius:4px ;display:block;margin:0 auto;width:50%;text-align:ceneter;text-decoration:none"">Activate Account</a>

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
