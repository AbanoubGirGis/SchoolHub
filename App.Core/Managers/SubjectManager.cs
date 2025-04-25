using App.Core.DTOs.SubjectDTOs;
using App.Core.Entities;
using App.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Managers
{
    public class SubjectManager
    {
        private readonly SchoolHubContext schoolHubContext;

        public SubjectManager(SchoolHubContext schoolHubContext)
        {
            this.schoolHubContext = schoolHubContext;
        }

        public async Task<Result<List<Subject>>> GetSubjects()
        {
            var subjects = await schoolHubContext.Subjects
                                .Include(w => w.Teacher)
                                .ToListAsync();
            return Result<List<Subject>>.Success(subjects);
        }

        public async Task<Result<Subject>> GetSubject(int Id)
        {
            var subject = await schoolHubContext.Subjects
                                .Include(w => w.Teacher)
                                .FirstOrDefaultAsync(w => w.Id == Id);
            if (subject == null)
            {
                return Result<Subject>.Failure("Subject not found");
            }
            return Result<Subject>.Success(subject);
        }

        public async Task<Result<List<Subject>>> GetUserSubjects(string userId)
        {
            var userSubject = await schoolHubContext.Users.FirstOrDefaultAsync(w => w.UserId == userId);
            if (userSubject == null)
            {
                return Result<List<Subject>>.Failure("User Subject not found");
            }

            var subjects = await schoolHubContext.Subjects
                                .Include(w => w.Teacher)
                                .Where(w => w.TeacherId == userSubject.Id)
                                .ToListAsync();
            if (subjects == null)
            {
                return Result<List<Subject>>.Failure("Subjects not found");
            }
            return Result<List<Subject>>.Success(subjects);
        }

        public async Task<Result<Subject>> CreateSubject(SubjectDTO SubjectDTO)
        {
            try
            {
                var Subject = new Subject
                {
                    TeacherId = SubjectDTO.TeacherId,
                    Name = SubjectDTO.Name,
                    CreatedAt = DateTime.Now,
                };
                await schoolHubContext.Subjects.AddAsync(Subject);
                await schoolHubContext.SaveChangesAsync();
                return Result<Subject>.Success(Subject);

            }
            catch (Exception ex)
            {
                return Result<Subject>.Failure($"Error creating Subject: {ex.Message}");
            }
        }

        public async Task<Result<Subject>> UpdateSubject(UpdateSubjectDTO updateSubjectDTO)
        {
            try
            {
                var subject = await schoolHubContext.Subjects.FirstOrDefaultAsync(w => w.Id == updateSubjectDTO.SubjectId);
                if (subject == null)
                {
                    return Result<Subject>.Failure("Subject not found");
                }
                subject.TeacherId = updateSubjectDTO.TeacherId;
                subject.Name = updateSubjectDTO.Name;
                subject.CreatedAt = DateTime.Now;

                schoolHubContext.Subjects.Update(subject);
                await schoolHubContext.SaveChangesAsync();
                return Result<Subject>.Success(subject);

            }
            catch (Exception ex)
            {
                return Result<Subject>.Failure($"Error update Subject: {ex.Message}");
            }
        }

        public async Task<Result<Subject>> DeleteSubject(int SubjectId)
        {
            try
            {
                var Subject = await schoolHubContext.Subjects.FirstOrDefaultAsync(w => w.Id == SubjectId);
                if (Subject == null)
                {
                    return Result<Subject>.Failure("Subject not found");
                }
                var grade = await schoolHubContext.Grades.FirstOrDefaultAsync(w => w.SubjectId == SubjectId);
                if (grade != null)
                {
                    grade.SubjectId = null;
                    schoolHubContext.Grades.Update(grade);
                }
                schoolHubContext.Subjects.Remove(Subject);
                await schoolHubContext.SaveChangesAsync();
                return Result<Subject>.Success(Subject);
            }
            catch (Exception ex)
            {
                return Result<Subject>.Failure($"Error deleting Subject: {ex.Message}");
            }
        }
    }
}
