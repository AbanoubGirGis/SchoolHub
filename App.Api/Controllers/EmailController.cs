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
        private readonly IEmailService iEmailService;
        private readonly UserManager userManager;

        public EmailController(IEmailService iEmailService, UserManager userManager)
        {
            this.iEmailService = iEmailService;
            this.userManager = userManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            var userResult = await userManager.CheckUser(request.UserId, request.Password!);
            if (!userResult.IsSuccess)
            {
                return NotFound(new { message = userResult.ErrorMessage });
            }

            var updatedUserResult = await UpdateUserOtp(userResult.Data);
            if (!updatedUserResult.IsSuccess)
            {
                return NotFound(new { message = updatedUserResult.ErrorMessage });
            }

            if (!await SendOtpEmail(updatedUserResult.Data.Email, (int)updatedUserResult.Data.Otp))
            {
                return BadRequest(new { message = "Failed to send OTP email" });
            }

            return Ok(new
            {
                userId = updatedUserResult.Data.UserId,
                message = "Email with OTP is sent"
            });
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

        private async Task<Result<User>> UpdateUserOtp(User user)
        {
            user.Otp = GenerateOTP();
            var updateResult = await userManager.UpdateUserOtp(user);

            if (!updateResult.IsSuccess)
            {
                return Result<User>.Failure("Error updating user OTP");
            }

            return Result<User>.Success(updateResult.Data);
        }

        private async Task<bool> SendOtpEmail(string email, int otp)
        {
            return await iEmailService.SendOtpEmail(email, otp.ToString());
        }

        private int GenerateOTP()
        {
            return new Random().Next(100000, 999999);
        }
    }
}