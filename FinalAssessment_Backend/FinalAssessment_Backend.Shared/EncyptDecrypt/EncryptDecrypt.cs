using Microsoft.AspNetCore.DataProtection;
using System;
using System.Text;

namespace FinalAssessment_Backend.Shared.EncryptDecrypt
{
    public class EncryptDecrypt
    {
        private readonly IDataProtector _dataProtector;

        public EncryptDecrypt(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("This is the secret key to protect");
        }

        public byte[] EncryptPlainText(string plainText)
        {
            //Converting plain text to binary then encrypting because in db i have taken varbinary
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return _dataProtector.Protect(plainBytes);
        }

        public string DecryptCipherText(byte[] cipherText)
        {
            var plainBytes = _dataProtector.Unprotect(cipherText);
            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}