
using FinalAssessment_Backend.Models.Dto;
using FinalAssessment_Backend.ServiceInterface;
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

            return Ok(res);
        }


        [HttpPost("RemoveUser/{id}")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            var res = await _userService.RemoveUserById(id);

            return Ok(new {success=res});
        }


        [HttpGet("GetRecords")]
        public async Task<IActionResult> GetRecords(int currentPage, int itemsPerPage)
        {
            var res = await _userService.GetPagedRecords(currentPage, itemsPerPage);

            return Ok(new {success = true, record=res.Records, totalRecords=res.TotalRecords});
        }
    }
}
