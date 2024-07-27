using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinalAssessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService) 
        { 
            _accountService = accountService;

        }


        [HttpPost("LoginUser")]
        public async Task<IActionResult> login(LoginCredentialsDto loginCredentials)
        {
            var res = await _accountService.loginUser(loginCredentials);


            if(res.success)
            {
                return Ok(new {success=res.success, message = res.msg, token=res.token});
            }

            return BadRequest(new { success = res.success, message = res.msg });
        }




        [HttpPost("ActivateUser/{userId}")]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            var res = await _accountService.ActivateAccount(userId);

            if(res == true)
            {
                return Ok(new { success = true, message = "Account is activated" });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = "Error activating account" }); ;
        }


        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var response = await _accountService.forgotPasswordService(email);

            if (response.success == true)
            {
                return Ok(new { success = response.success, message = response.msg });
            }
            
            return BadRequest(new { success = response.success, message=response.msg });
        }


        [HttpPost("ResetPassword")]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var res = await _accountService.resetPasswordService(dto, token);

            if (res.success)
            {
                return Ok(new {success = res.success, message = res.msg});
            }

            return BadRequest(new {success = res.success, message = res.msg });
        }


        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string pass)
        {
            return Ok(pass);
        }
    }
}
