using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Shared.Hashing
{
    public class Hashing : IHashing
    {
        public Hashing() { }


        public string GenerateHash(string password)
        {
            //Bcrypt by default uses SHA384 algo
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, BCrypt.Net.HashType.SHA512);
            return hashedPassword;
        }


        public bool VerifyHash(string loginPassword, string hashedPasswordDb)
        {
            var result = BCrypt.Net.BCrypt.EnhancedVerify(loginPassword, hashedPasswordDb, BCrypt.Net.HashType.SHA512);
            return result;
        }
    }
}
