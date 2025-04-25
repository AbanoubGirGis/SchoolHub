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
using App.Core.DTOs.SubjectDTOs;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private readonly SubjectManager subjectManager;

        public SubjectController(SubjectManager subjectManager)
        {
            this.subjectManager = subjectManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjects()
        {
            var subjectResult = await subjectManager.GetSubjects();
            if (!subjectResult.IsSuccess)
            {
                return StatusCode(500, subjectResult.ErrorMessage);
            }
            return Ok(new SubjectsDTO
            {
                Subjects = subjectResult.Data.ToList(),
                Count = subjectResult.Data.Count()
            });
        }

        [HttpGet("{subjectId}")]
        public async Task<IActionResult> GetSubject(int subjectId)
        {
            var subjectResult = await subjectManager.GetSubject(subjectId);
            if (!subjectResult.IsSuccess)
            {
                return NotFound(subjectResult.ErrorMessage);
            }
            return Ok(subjectResult.Data);
        }

        [HttpPost()]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectDTO subjectDTO)
        {
            var subjectResult = await subjectManager.CreateSubject(subjectDTO);
            if (!subjectResult.IsSuccess) 
            {
                return StatusCode(500, subjectResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" Subject ID {subjectResult.Data.Id} has been successfully created."
            });
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateSubject([FromBody] UpdateSubjectDTO updateSubjectDTO)
        {
            var subjectResult = await subjectManager.UpdateSubject(updateSubjectDTO);
            if (!subjectResult.IsSuccess)
            {
                return StatusCode(500, subjectResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" Subject ID {subjectResult.Data.Id} has been successfully updated."
            });
        }

        [HttpDelete("{subjectId}")]
        public async Task<IActionResult> DeleteSubject(int subjectId)
        {
            var subjectResult = await subjectManager.DeleteSubject(subjectId);
            if (!subjectResult.IsSuccess)
            {
                return StatusCode(500, subjectResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" Subject ID {subjectResult.Data.Id} has been successfully deleted."
            });
        }

        [HttpGet("userSubjects/{userId}")]
        public async Task<IActionResult> GetUserSubjects(string userId)
        {
            var subjectResult = await subjectManager.GetUserSubjects(userId);
            if (!subjectResult.IsSuccess)
            {
                return NotFound(subjectResult.ErrorMessage);
            }
            return Ok(new SubjectsDTO
            {
                Subjects = subjectResult.Data.ToList(),
                Count = subjectResult.Data.Count()
            });
        }
    }
}