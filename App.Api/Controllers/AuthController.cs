using App.Core.DTOs;
using App.Core.Entities;
using App.Core.Infrastructure;
using App.Core.Managers;
using App.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager _userManager;
        private readonly IEmailService _emailService;

        public AuthController(
            IConfiguration configuration,
            UserManager userManager,
            IEmailService emailService)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (model == null || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid login request");
            }

            var userResult = await _userManager.CheckUser(model.UserId, model.Password);
            if (!userResult.IsSuccess)
            {
                return NotFound("Invalid Username or Password, Try again");
            }

            var updatedUserResult = await UpdateUserOtp(userResult.Data);
            if (!updatedUserResult.IsSuccess)
            {
                return StatusCode(500, "Error processing authentication");
            }

            var isSent = await SendOtpEmail(updatedUserResult.Data.Email, (int)updatedUserResult.Data.Otp);
            if (!isSent)
            {
                return StatusCode(500, "Failed to send OTP email");
            }

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

            var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey")
                ?? throw new InvalidOperationException("JWT secret key is not configured");

            var token = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("JwtSettings:Issuer"),
                audience: _configuration.GetValue<string>("JwtSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        private async Task<bool> SendOtpEmail(string email, int otp)
        {
            return await _emailService.SendOtpEmail(email, otp.ToString());
        }

        private async Task<Result<User>> UpdateUserOtp(User user)
        {
            user.Otp = GenerateOtp();
            var updateResult = await _userManager.UpdateUserOtp(user);

            return updateResult.IsSuccess
                ? Result<User>.Success(updateResult.Data)
                : Result<User>.Failure("Error updating user OTP");
        }

        private static int GenerateOtp()
        {
            return new Random().Next(100000, 999999);
        }
    }
}