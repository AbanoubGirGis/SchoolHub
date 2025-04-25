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
                var users = await schoolHubContext.Users.ToListAsync();
                var teacherId = users.FirstOrDefault(x => x.UserId == dto.TeacherId)?.Id;
                var studentId = users.FirstOrDefault(x => x.UserId == dto.StudentId)?.Id;
                var grade = new Grade
                {
                    TeacherId = teacherId,
                    StudentId = studentId,
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

                var users = await schoolHubContext.Users.ToListAsync();
                var teacherId = users.FirstOrDefault(x => x.UserId == dto.TeacherId)?.Id;
                var studentId = users.FirstOrDefault(x => x.UserId == dto.StudentId)?.Id;

                grade.TeacherId = teacherId;
                grade.StudentId = studentId;
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

        public async Task<Result<List<Grade>>> GetUserGrades(string userId)
        {
            var userGrade = await schoolHubContext.Users.FirstOrDefaultAsync(w => w.UserId == userId);
            if (userGrade == null)
            {
                return Result<List<Grade>>.Failure("User's grade not found");
            }

            var userGrades = await schoolHubContext.Grades
                                .Include(w => w.Student)
                                .Include(s => s.Subject)
                                .Include(t => t.Teacher)
                                .Where(w => w.StudentId == userGrade.Id)
                                .ToListAsync();

            if (userGrades == null)
            {
                return Result<List<Grade>>.Failure("User grades not found");
            }
            return Result<List<Grade>>.Success(userGrades);
        }
    }
}