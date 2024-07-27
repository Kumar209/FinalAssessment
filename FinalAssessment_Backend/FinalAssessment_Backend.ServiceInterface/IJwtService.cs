using FinalAssessment_Backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.ServiceInterface
{
    public interface IJwtService
    {
        public Task<string> GenerateToken(PrashantDbUser user);

        public Task<int> ValidateJwtToken(string token);
    }
}
