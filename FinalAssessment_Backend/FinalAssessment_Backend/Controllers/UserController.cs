
using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.ServiceInterface;
using FinalAssessment_Backend.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalAssessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        public UserController(IUserService userService , ILogger<UserController> logger) 
        { 
            _logger = logger;
            _logger.LogDebug("Nlog is integrated to user controller");
            _userService = userService;
        }


        [HttpPost("AddUser")]
        [Authorize]
        public async Task<IActionResult> AddUser([FromForm] PrashantDbUserDto userDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success=false, message = ResponseMessage.validationFailed});
            }

            try
            {
                var res = await _userService.CreateUser(userDetails);

                return Ok(new { success = res, message = ResponseMessage.addedUserSuccess });
            }
            

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
            }
        }


        [HttpDelete("RemoveUser/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveUser(int id)
        {
            try
            {
                var res = await _userService.RemoveUserById(id);

                return Ok(new { success = res, message = ResponseMessage.deleteUserSuccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
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
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
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
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
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
                _logger.LogError(ex.Message);
                return StatusCode(500, new {success=false, message=ResponseMessage.internalServerError, error=ex.Message});
            }
        }




        [HttpPut("UpdateUser")]
        [Authorize]

        public async Task<IActionResult> UpdateUser([FromForm] PrashantDbUserDto userDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = ResponseMessage.validationFailed });
            }

            try
            {
                var res = await _userService.UpdateUserDetails(userDetails);

                return Ok(new { success = res, message = ResponseMessage.updateUserSuccess });
            }


            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
            }
        }



    }
}
