
using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalAssessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) 
        { 
            _userService = userService;
        }


        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromForm] PrashantDbUserDto userDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var res = await _userService.CreateUser(userDetails);

            return Ok(new {success=res, message="Successfully created user and credential send to email"});
        }


        [HttpDelete("RemoveUser/{id}")]

        public async Task<IActionResult> RemoveUser(int id)
        {
            var res = await _userService.RemoveUserById(id);

            return Ok(new {success=res, message="User is deleted"});
        }


        [HttpGet("GetRecords")]

        public async Task<IActionResult> GetRecords([FromQuery] UserQueryParams userQuery)
        {
            var res = await _userService.GetPagedRecords(userQuery);

            return Ok(new {success = true, record=res.Records, TotalUsersCount = res.TotalUsersCount , TotalActiveCount = res.TotalActiveCount , TotalInactiveCount = res.TotalInactiveCount });
        }


        [HttpGet("DownloadExcel")]
        public async Task<IActionResult> DownloadExcel()
        {
            var excelData = await _userService.GenerateExcelAsync();

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "Users.xlsx";

            return File(excelData, contentType, fileName);
        }


    }
}
