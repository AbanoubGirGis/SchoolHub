using App.Core.DTOs.GradeDTOs;
using App.Core.Entities;
using App.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace App.Core.Managers
{
    public class GradeManager
    {
        private readonly SchoolHubContext schoolHubContext;

        public GradeManager(SchoolHubContext context)
        {
            this.schoolHubContext = context;
        }

        public async Task<Result<List<Grade>>> GetGrades()
        {
            try
            {
                var grades = await schoolHubContext.Grades
                    .Include(g => g.Teacher)
                    .Include(g => g.Student)
                    .Include(g => g.Subject)
                    .ToListAsync();
                return Result<List<Grade>>.Success(grades);
            }
            catch (Exception ex)
            {
                return Result<List<Grade>>.Failure($"Error retrieving grades: {ex.Message}");
            }
        }

        public async Task<Result<Grade>> GetGrade(int id)
        {
            try
            {
                var grade = await schoolHubContext.Grades
                    .Include(g => g.Teacher)
                    .Include(g => g.Student)
                    .Include(g => g.Subject)
                    .FirstOrDefaultAsync(g => g.Id == id);
                return grade != null
                    ? Result<Grade>.Success(grade)
                    : Result<Grade>.Failure("Grade not found.");
            }
            catch (Exception ex)
            {
                return Result<Grade>.Failure($"Error retrieving grade: {ex.Message}");
            }
        }

        public async Task<Result<Grade>> CreateGrade(CreateGradeDTO dto)
        {
            try
            {
                var grade = new Grade
                {
                    TeacherId = dto.TeacherId,
                    StudentId = dto.StudentId,
                    SubjectId = dto.SubjectId,
                    ExamType = dto.ExamType,
                    Score = dto.Score,
                    MaxScore = dto.MaxScore,
                    CreatedAt = DateTime.Now
                };
                await schoolHubContext.Grades.AddAsync(grade);
                await schoolHubContext.SaveChangesAsync();
                return Result<Grade>.Success(grade);
            }
            catch (Exception ex)
            {
                return Result<Grade>.Failure($"Error creating grade: {ex.Message}");
            }
        }

        public async Task<Result<Grade>> UpdateGrade(UpdateCreateGradeDTO dto)
        {
            try
            {
                var grade = await schoolHubContext.Grades.FindAsync(dto.Id);
                if (grade == null)
                    return Result<Grade>.Failure("Grade not found.");

                grade.TeacherId = dto.TeacherId;
                grade.StudentId = dto.StudentId;
                grade.SubjectId = dto.SubjectId;
                grade.ExamType = dto.ExamType;
                grade.Score = dto.Score;
                grade.MaxScore = dto.MaxScore;
                grade.CreatedAt = DateTime.Now;

                schoolHubContext.Grades.Update(grade);
                await schoolHubContext.SaveChangesAsync();
                return Result<Grade>.Success(grade);
            }
            catch (Exception ex)
            {
                return Result<Grade>.Failure($"Error updating grade: {ex.Message}");
            }
        }

        public async Task<Result<Grade>> DeleteGrade(int id)
        {
            try
            {
                var grade = await schoolHubContext.Grades.FindAsync(id);
                if (grade == null)
                    return Result<Grade>.Failure("Grade not found.");

                schoolHubContext.Grades.Remove(grade);
                await schoolHubContext.SaveChangesAsync();
                return Result<Grade>.Success(grade);
            }
            catch (Exception ex)
            {
                return Result<Grade>.Failure($"Error deleting grade: {ex.Message}");
            }
        }
    }
}
