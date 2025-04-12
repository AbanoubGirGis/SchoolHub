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

        public UserController(UserManager userManager)
        {
            this.userManager = userManager;
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
        public async Task<IActionResult> CreateUser([FromForm] UserDTO userDTO)
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
        public async Task<IActionResult> UpdateUser([FromForm] UserDTO userDTO)
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
                message = $" User ID {userResult.Data.UserId} has been successfully updated."
            });
        }
    }
}