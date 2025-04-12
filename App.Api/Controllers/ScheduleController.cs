using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using App.Core.Managers;
using App.Core.DTOs;
using App.Core.Services;
using App.Core.Entities;
using App.Core.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ScheduleManager scheduleManager;

        public ScheduleController(IWebHostEnvironment webHostEnvironment, ScheduleManager scheduleManager)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.scheduleManager = scheduleManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchedules()
        {
            var schdulesResult = await scheduleManager.GetSchedules();
            if (!schdulesResult.IsSuccess)
            {
                return StatusCode(500, schdulesResult.ErrorMessage);
            }
            return Ok(new
            {
                Schedules = schdulesResult.Data,
                ShedulesCount = schdulesResult.Data.Count()
            });
        }

        [HttpGet("{scheduleId}")]
        public async Task<IActionResult> GetScheduleById(int scheduleId)
        {
            var result = await scheduleManager.GetSchedule(scheduleId);
            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateSchedule([FromForm] ScheduleDTO scheduleDTO)
        {
            if (scheduleDTO == null || scheduleDTO.FormFile?.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Path.GetFileName(scheduleDTO.FormFile!.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await scheduleDTO.FormFile.CopyToAsync(stream);
            }

            var scheduleResult = await scheduleManager.CreateSchedule(fileName, scheduleDTO.UserTypeId);
            if (!scheduleResult.IsSuccess)
                return BadRequest(scheduleResult.ErrorMessage);

            return Ok(new
            {
                message = $"The schedule for UserType ID {scheduleResult.Data.UserTypeId} has been successfully created."
            });
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateSchedule([FromForm] ScheduleDTO scheduleDTO)
        {
            if (scheduleDTO == null || scheduleDTO.FormFile?.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Path.GetFileName(scheduleDTO.FormFile!.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await scheduleDTO.FormFile.CopyToAsync(stream);
            }

            var scheduleResult = await scheduleManager.UpdateSchedule(scheduleDTO.ScheduleId,fileName, scheduleDTO.UserTypeId);
            if (!scheduleResult.IsSuccess)
                return BadRequest(scheduleResult.ErrorMessage);

            return Ok(new
            {
                message = $"The schedule for UserType ID {scheduleResult.Data.UserTypeId} has been successfully updated."
            });
        }

        [HttpDelete("{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(int scheduleId)
        {
            var result = await scheduleManager.DeleteSchedule(scheduleId);
            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(new { message = "Schedule deleted successfully." });
        }
    }
}