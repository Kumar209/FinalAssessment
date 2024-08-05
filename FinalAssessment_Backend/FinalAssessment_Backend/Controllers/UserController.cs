
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
        [Authorize]
        public async Task<IActionResult> AddUser([FromForm] PrashantDbUserDto userDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success=false, message = "Validation failed"});
            }

            try
            {
                var res = await _userService.CreateUser(userDetails);

                return Ok(new { success = res, message = "Successfully created user and credential send to email" });
            }
            

            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while creating the user.", error = ex.Message });
            }
        }


        [HttpDelete("RemoveUser/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveUser(int id)
        {
            try
            {
                var res = await _userService.RemoveUserById(id);

                return Ok(new { success = res, message = "User is deleted" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while deleting the user.", error = ex.Message });
            }
        }


        [HttpGet("GetRecords")]
        [Authorize]
        public async Task<IActionResult> GetRecords([FromQuery] UserQueryParams userQuery)
        {
            try
            {
                var res = await _userService.GetPagedRecords(userQuery);

                return Ok(new { success = true, record = res.Records, TotalUsersCount = res.TotalUsersCount, TotalActiveCount = res.TotalActiveCount, TotalInactiveCount = res.TotalInactiveCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while getting records.", error = ex.Message });
            }
           
        }


        [HttpGet("DownloadExcel")]
        [Authorize]
        public async Task<IActionResult> DownloadExcel()
        {
            try
            {
                var excelData = await _userService.GenerateExcelAsync();

                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var fileName = "Users.xlsx";

                return File(excelData, contentType, fileName);
            }

            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while exporting excel.", error = ex.Message });
            }
            
        }


        [HttpGet("GetUserById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var res = await _userService.GetUserDetailById(id);

                return Ok(new { success = true, record = res });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new {success=false, message="Error occurred while getting user", error=ex.Message});
            }
        }




        [HttpPut("UpdateUser")]
        [Authorize]

        public async Task<IActionResult> UpdateUser([FromForm] PrashantDbUserDto userDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed" });
            }

            try
            {
                var res = await _userService.UpdateUserDetails(userDetails);

                return Ok(new { success = res, message = "Successfully updated user" });
            }


            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while updating the user.", error = ex.Message });
            }
        }



    }
}
