using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.ServiceInterface;
using FinalAssessment_Backend.Shared.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace FinalAssessment_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger) 
        { 
            _logger = logger;
            _logger.LogDebug("Nlog is integrated to account controller");
            _accountService = accountService;

        }


        [HttpPost("LoginUser")]
        public async Task<IActionResult> login(LoginCredentialsDto loginCredentials)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Validation failed");
                return BadRequest(new { success = false, message = ResponseMessage.validationFailed });
            }

            try
            {
                var res = await _accountService.loginUser(loginCredentials);


                if (res.success)
                {
                    return Ok(new { success = res.success, message = res.msg, token = res.token, requiredDataForFrontend = res.requiredDataForFrontend });
                }

                return BadRequest(new { success = res.success, message = res.msg });
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
            }

            
        }




        [HttpPatch("ActivateUser")]
        [Authorize]
        public async Task<IActionResult> ActivateUser()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { success = false, message = ResponseMessage.unauthorizeUser });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var res = await _accountService.ActivateAccount(token);


            try
            {
                if (res.success)
                {
                    return Ok(new { success = res.success, message = res.msg });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new { success = res.success, message = res.msg });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
            }
            
        }



        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var response = await _accountService.forgotPasswordService(email);

            try
            {
                if (response.success == true)
                {
                    return Ok(new { success = response.success, message = response.msg });
                }

                return BadRequest(new { success = response.success, message = response.msg });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
            }
        }



        [HttpPost("ResetPassword")]
        [Authorize]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed" });
            }

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { success = false, message = ResponseMessage.unauthorizeUser });
            }

            try
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                var res = await _accountService.resetPasswordService(dto, token);

                if (res.success)
                {
                    return Ok(new { success = res.success, message = res.msg });
                }

                return BadRequest(new { success = res.success, message = res.msg });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
            }
        }



        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { success = false, message = ResponseMessage.unauthorizeUser });
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();


            try
            {
                var res = await _accountService.changePasswordService(dto, token);

                if (res.success)
                {
                    return Ok(new { success = res.success, message = res.msg });
                }

                return BadRequest(new { success = res.success, message = res.msg });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new { success = false, message = ResponseMessage.internalServerError, error = ex.Message });
            }
        }
    }
 }
