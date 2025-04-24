using App.Core.DTOs;
using App.Core.Entities;
using App.Core.Infrastructure;
using App.Core.Managers;
using App.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly UserManager userManager;
        private readonly IEmailService emailService;
        private readonly OtpManager otpManager;

        public AuthController(
            IConfiguration configuration,
            UserManager userManager,
            IEmailService emailService,
            OtpManager otpManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            this.emailService = emailService;
            this.otpManager = otpManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            // Check UserId & UserPassword
            var userResult = await userManager.VerifyUserCredentials(model.UserId, model.Password);
            if (!userResult.IsSuccess)
            {
                return NotFound("Invalid Username or Password, Try again");
            }

            // Send Otp
            var isSent = await SendOtp(userResult.Data);
            if (!isSent.IsSuccess)
            {
                return StatusCode(500, isSent.ErrorMessage);
            }

            // Generate Token
            var token = GenerateAccessToken(model.UserId.ToString());
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                accessToken = tokenString,
                userId = userResult.Data.UserId,
                message = "An email containing the OTP has been sent. Please check your inbox."
            });
        }

        private JwtSecurityToken GenerateAccessToken(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
            };

            var secretKey = configuration.GetValue<string>("JwtSettings:SecretKey")
                ?? throw new InvalidOperationException("JWT secret key is not configured");

            var token = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("JwtSettings:Issuer"),
                audience: configuration.GetValue<string>("JwtSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        private async Task<Result<bool>> SendOtp(User user)
        {
            var otpResult = await otpManager.CreateOrUpdateUserOtp(user.Id);
            if(!otpResult.IsSuccess)
            {
                return Result<bool>.Failure(otpResult.ErrorMessage);
            }

            var isSent = await emailService.SendOtpEmail(user.Email, Convert.ToString(otpResult.Data.Code)!);
            if (!isSent)
            {
                return Result<bool>.Failure("Failed to send OTP email");
            }
            return Result<bool>.Success(isSent);
        }
    }
}