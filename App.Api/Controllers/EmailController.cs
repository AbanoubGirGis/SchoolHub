using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Core.Managers;
using App.Core.DTOs;
using App.Core.Services;
using App.Core.Entities;
using App.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailController : ControllerBase
    {
        private readonly UserManager userManager;
        public EmailController( UserManager userManager)
        {
            this.userManager = userManager;
        }
        [HttpPost("CheckOTP")]
        public async Task<IActionResult> CheckOtp(int userId, int otp)
        {
            var checkOtp = await userManager.CheckUserOTP(userId, otp);
            if(!checkOtp.IsSuccess)
            {
                return BadRequest(new { message = checkOtp.ErrorMessage });
            }
            return Ok(new
            {
                userTypeId = checkOtp.Data.UserType,
                userType = checkOtp.Data.UserTypeNavigation.Type,
            });
        }
    }
}