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
using App.Core.DTOs.WarningDTOs;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WarningController : ControllerBase
    {
        private readonly WarningManager warningManager;

        public WarningController(WarningManager warningManager)
        {
            this.warningManager = warningManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetWarnings()
        {
            var warningResult = await warningManager.GetWarnings();
            if (!warningResult.IsSuccess)
            {
                return StatusCode(500, warningResult.ErrorMessage);
            }
            return Ok(new WarningsDTO
            {
                Warnings = warningResult.Data.ToList(),
                Count = warningResult.Data.Count()
            });
        }

        [HttpGet("{warningId}")]
        public async Task<IActionResult> GetWarning(int warningId)
        {
            var warningResult = await warningManager.GetWarning(warningId);
            if (!warningResult.IsSuccess)
            {
                return NotFound(warningResult.ErrorMessage);
            }
            return Ok(warningResult.Data);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateWarning([FromBody] WarningDTO warningDTO)
        {
            var warningResult = await warningManager.CreateWarning(warningDTO);
            if (!warningResult.IsSuccess)
            {
                return StatusCode(500, warningResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" Warning ID {warningResult.Data.Id} has been successfully created."
            });
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateWarning([FromBody] UpdateWarningDTO updateWarningDTO)
        {
            var warningResult = await warningManager.UpdateWarning(updateWarningDTO);
            if (!warningResult.IsSuccess)
            {
                return StatusCode(500, warningResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" Warning ID {warningResult.Data.Id} has been successfully updated."
            });
        }

        [HttpDelete("{warningId}")]
        public async Task<IActionResult> DeleteWarning(int warningId)
        {
            var warningResult = await warningManager.DeleteWarning(warningId);
            if (!warningResult.IsSuccess)
            {
                return StatusCode(500, warningResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" Warning ID {warningResult.Data.Id} has been successfully updated."
            });
        }

        [HttpGet("userWarning/{userId}")]
        public async Task<IActionResult> GetUserWarning(string userId)
        {
            var warningResult = await warningManager.GetUserWarnings(userId);
            if (!warningResult.IsSuccess)
            {
                return NotFound(warningResult.ErrorMessage);
            }
            return Ok(new WarningsDTO
            {
                Warnings = warningResult.Data.ToList(),
                Count = warningResult.Data.Count()
            });
        }
    }
}