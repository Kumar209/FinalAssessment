using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.ServiceInterface
{
    public interface IHashing
    {
        public string GenerateHash(string password);
        public bool VerifyHash(string loginPassword, string hashedPasswordDb);
    }
}
