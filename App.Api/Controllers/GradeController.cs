using App.Core.DTOs.GradeDTOs;
using App.Core.DTOs.UsersDTOs;
using App.Core.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GradeController : ControllerBase
    {
        private readonly GradeManager gradeManager;

        public GradeController(GradeManager gradeManager)
        {
            this.gradeManager = gradeManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetGrades()
        {
            var result = await gradeManager.GetGrades();
            return result.IsSuccess ? Ok( new GradesDTO
            {
                Grades = result.Data,
                Count = result.Data.Count
            }) : StatusCode(500, result.ErrorMessage);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGrade(int id)
        {
            var result = await gradeManager.GetGrade(id);
            return result.IsSuccess ? Ok(result.Data) : NotFound(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGrade([FromBody] CreateGradeDTO dto)
        {
            var result = await gradeManager.CreateGrade(dto);
            return result.IsSuccess
                ? Ok(new { message = "Grade created successfully", grade = result.Data })
                : StatusCode(500, result.ErrorMessage);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGrade([FromBody] UpdateCreateGradeDTO dto)
        {
            var result = await gradeManager.UpdateGrade(dto);
            return result.IsSuccess
                ? Ok(new { message = "Grade updated successfully", grade = result.Data })
                : StatusCode(500, result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var result = await gradeManager.DeleteGrade(id);
            return result.IsSuccess
                ? Ok(new { message = "Grade deleted successfully", grade = result.Data })
                : StatusCode(500, result.ErrorMessage);
        }

        [HttpGet("usergrade/{userId}")]
        public async Task<IActionResult> GetUsergrade(string userId)
        {
            var userGradeResult = await gradeManager.GetUserGrades(userId);
            if (!userGradeResult.IsSuccess)
            {
                return NotFound(userGradeResult.ErrorMessage);
            }
            return Ok(new GradesDTO
            {
                Grades = userGradeResult.Data.ToList(),
                Count = userGradeResult.Data.Count()
            });
        }
    }
}