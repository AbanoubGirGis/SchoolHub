using App.Core.DTOs.WarningDTOs;
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
    public class WarningManager
    {
        private readonly SchoolHubContext schoolHubContext;

        public WarningManager(SchoolHubContext schoolHubContext)
        {
            this.schoolHubContext = schoolHubContext;
        }

        public async Task<Result<List<Warning>>> GetWarnings()
        {
            var warnings = await schoolHubContext.Warnings
                                .Include(w => w.Student)
                                .Include(w => w.Subject)
                                .ToListAsync();
            return Result<List<Warning>>.Success(warnings);
        }

        public async Task<Result<Warning>> GetWarning(int Id)
        {
            var warning = await schoolHubContext.Warnings
                                .Include(w => w.Student)
                                .Include(w => w.Subject)
                                .FirstOrDefaultAsync(w => w.Id == Id);
            if (warning == null)
            {
                return Result<Warning>.Failure("Warning not found");
            }
            return Result<Warning>.Success(warning);
        }

        public async Task<Result<Warning>> GetUserWarning(string userId)
        {
            var userWarning = await schoolHubContext.Users.FirstOrDefaultAsync(w => w.UserId == userId);
            if (userWarning == null)
            {
                return Result<Warning>.Failure("User Warning not found");
            }

            var warning = await schoolHubContext.Warnings
                                .Include(w => w.Student)
                                .FirstOrDefaultAsync(w => w.StudentId == userWarning.Id);
            if (warning == null)
            {
                return Result<Warning>.Failure("Warning not found");
            }
            return Result<Warning>.Success(warning);
        }

        public async Task<Result<Warning>> CreateWarning(WarningDTO warningDTO)
        {
            try
            {
                var userWarning = await schoolHubContext.Users.FirstOrDefaultAsync(w => w.UserId == warningDTO.StudentId);
                if (userWarning == null)
                {
                    return Result<Warning>.Failure("User Warning not found");
                }

                var warning = new Warning
                {
                    SubjectId = warningDTO.SubjectId,
                    StudentId = userWarning.Id,
                    Reason = warningDTO.Reason,
                    CreatedAt = DateTime.Now,
                };
                await schoolHubContext.Warnings.AddAsync(warning);
                await schoolHubContext.SaveChangesAsync();
                return Result<Warning>.Success(warning);

            }
            catch (Exception ex)
            {
                return Result<Warning>.Failure($"Error creating warning: {ex.Message}");
            }
        }

        public async Task<Result<Warning>> UpdateWarning(UpdateWarningDTO updateWarningDTO)
        {
            try
            {
                var warning = await schoolHubContext.Warnings.FirstOrDefaultAsync(w => w.Id == updateWarningDTO.WarningId);
                var student = await schoolHubContext.Users.FirstOrDefaultAsync(u => u.UserId == updateWarningDTO.StudentId);
                if (warning == null || student == null)
                {
                    return Result<Warning>.Failure("Warning or Student not found");
                }

                warning.SubjectId = updateWarningDTO.SubjectId;
                warning.StudentId = student.Id;
                warning.Reason = updateWarningDTO.Reason;
                warning.CreatedAt = DateTime.Now;

                schoolHubContext.Warnings.Update(warning);
                await schoolHubContext.SaveChangesAsync();
                return Result<Warning>.Success(warning);

            }
            catch (Exception ex)
            {
                return Result<Warning>.Failure($"Error update warning: {ex.Message}");
            }
        }

        public async Task<Result<Warning>> DeleteWarning(int warningId)
        {
            try
            {

                var warning = await schoolHubContext.Warnings.FirstOrDefaultAsync(w => w.Id == warningId);
                if (warning == null)
                {
                    return Result<Warning>.Failure("Warning not found");
                }
                schoolHubContext.Warnings.Remove(warning);
                await schoolHubContext.SaveChangesAsync();
                return Result<Warning>.Success(warning);
            }
            catch (Exception ex)
            {
                return Result<Warning>.Failure($"Error deleting Warning: {ex.Message}");
            }
        }
    }
}
