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
using App.Core.DTOs.ParentStudentDTOs;

namespace App.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ParentStudentController : ControllerBase
    {
        private readonly ParentStudentManager parentStudentManager;

        public ParentStudentController(ParentStudentManager parentStudentManager)
        {
            this.parentStudentManager = parentStudentManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStudentsAndParents()
        {
            var studentsAndParentsResult = await parentStudentManager.GetAllStudentsParents();
            if (!studentsAndParentsResult.IsSuccess)
            {
                return StatusCode(500, studentsAndParentsResult.ErrorMessage);
            }
            return Ok(new ParentStudentDTO
            {
                ParentStudents = studentsAndParentsResult.Data.ToList(),
                Count = studentsAndParentsResult.Data.Count()
            });
        }

        [HttpGet("{parentId}")]
        public async Task<IActionResult> GetStudentByParentId(string parentId)
        {
            var studentResult = await parentStudentManager.GetStudentByParentId(parentId);
            if (!studentResult.IsSuccess)
            {
                return NotFound(studentResult.ErrorMessage);
            }
            return Ok(new
            {
                student = studentResult.Data
            });
        }

        [HttpPost()]
        public async Task<IActionResult> AssignStudentToParent(string parentId, string studentId)
        {
            var studentToParentResult = await parentStudentManager.AssignStudentToParent(parentId, studentId);
            if (!studentToParentResult.IsSuccess)
            {
                return NotFound(studentToParentResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" Assign Student to Parent has been successfully created."
            });
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateStudentToParent(string parentId, string studentId)
        {
            var studentToParentResult = await parentStudentManager.UpdateStudentToParent(parentId, studentId);
            if (!studentToParentResult.IsSuccess)
            {
                return NotFound(studentToParentResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" Student to Parent has been successfully updated."
            });
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteStudentByParentId(string parentId)
        {
            var studentToParentResult = await parentStudentManager.DeleteStudentByParentId(parentId);
            if (!studentToParentResult.IsSuccess)
            {
                return NotFound(studentToParentResult.ErrorMessage);
            }
            return Ok(new
            {
                message = $" Student to Parent has been successfully deleted."
            });
        }
    }
}