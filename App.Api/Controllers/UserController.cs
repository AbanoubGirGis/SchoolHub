using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Core.Managers;
using App.Core.Services;
using App.Core.Entities;
using App.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using App.Core.DTOs.SchedulesDTOs;
using App.Core.DTOs.UsersDTOs;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager userManager;
        private readonly IEmailService emailService;

        public UserController(UserManager userManager, IEmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var usersResult = await userManager.GetUsers();
            if (!usersResult.IsSuccess)
            {
                return StatusCode(500, usersResult.ErrorMessage);
            }
            return Ok(new UsersDTO
            {
                Users = usersResult.Data.ToList(),
                Count = usersResult.Data.Count()
            });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var result = await userManager.GetUser(userId);
            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
        {
            var userResult = await userManager.CreateUser(userDTO);
            if (!userResult.IsSuccess) 
            {
                return StatusCode(500, userResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" User ID {userResult.Data.UserId} has been successfully created."
            });
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateUser([FromBody] UserDTO userDTO)
        {
            var userResult = await userManager.UpdateUser(userDTO);
            if (!userResult.IsSuccess)
            {
                return StatusCode(500, userResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" User ID {userResult.Data.UserId} has been successfully updated."
            });
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var userResult = await userManager.DeleteUser(userId);
            if (!userResult.IsSuccess)
            {
                return StatusCode(500, userResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" User ID {userResult.Data.UserId} has been successfully deleted."
            });
        }

        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers()
        {
            var usersResult = await userManager.GetTeachers();
            if (!usersResult.IsSuccess)
            {
                return StatusCode(500, usersResult.ErrorMessage);
            }
            return Ok(new UsersDTO
            {
                Users = usersResult.Data.ToList(),
                Count = usersResult.Data.Count()
            });
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> ForgetPassword(string userId)
        {
            var userResult = await userManager.ForgetPassword(userId);
            if (!userResult.IsSuccess)
            {
                return StatusCode(500, userResult.ErrorMessage);
            }

            var isSent = await emailService.SendNewPassword(userResult.Data.Email, userResult.Data.Password);
            if (!isSent)
            {
                return StatusCode(500, "Failed to send New Password");
            }

            return Ok(new
            {
                message = "An email containing the New Password has been sent. Please check your inbox."
            });
        }
    }
}