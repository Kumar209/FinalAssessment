using FinalAssessment_Backend.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.ServiceInterface
{
    public interface IUserService
    {
        public Task<bool> CreateUser(PrashantDbUserDto userDetails);

        public Task<bool> RemoveUserById(int id);

        public Task<PageRecordDto> GetPagedRecords(UserQueryParams userQuery);

        public Task<byte[]> GenerateExcelAsync();

        public Task<PrashantDbUserDto> GetUserDetailById(int id);

    }
}
