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
        private readonly OtpManager otpManager;

        public EmailController(OtpManager otpManager)
        {
            this.otpManager = otpManager;
        }
        [HttpPost("CheckOTP")]
        public async Task<IActionResult> CheckOtp(string userId, int otp)
        {
            var checkOtp = await otpManager.CheckUserOTP(userId, otp);
            if(!checkOtp.IsSuccess)
            {
                return BadRequest(new { message = checkOtp.ErrorMessage });
            }
            return Ok(new UserDTO
            {
                UserId = checkOtp.Data.User.UserId,
                UserType = checkOtp.Data.User.UserType.TypeName,
                FullName = checkOtp.Data.User.FullName,
                Email = checkOtp.Data.User.Email,
                Password = checkOtp.Data.User.Password,
            });
        }
    }
}