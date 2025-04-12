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
    public class ScheduleManager
    {
        private readonly SchoolHubContext schoolHubContext;

        public ScheduleManager(SchoolHubContext schoolHubContext)
        {
            this.schoolHubContext = schoolHubContext;
        }

        // for Admin
        public async Task<Result<List<Schedule>>> GetSchedules()
        {
            try
            {
                var schedules = await schoolHubContext.Schedules
                    .Include(s => s.UserType)
                    .ToListAsync();
                if (schedules == null)
                {
                    return Result<List<Schedule>>.Failure("Fail to get Schedules");
                }
                return Result<List<Schedule>>.Success(schedules);
            }
            catch (Exception ex)
            {
                return Result<List<Schedule>>.Failure($"An error occurred when get all Schedules: {ex.Message}");
            }
        }
        public async Task<Result<Schedule>> GetSchedule(int scheduleId)
        {
            try
            {
                var schedule = await schoolHubContext.Schedules
                    .Include(s => s.UserType)
                    .FirstOrDefaultAsync(x => x.Id == scheduleId);
                if (schedule == null)
                {
                    return Result<Schedule>.Failure("Schedule not found");
                }
                return Result<Schedule>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<Schedule>.Failure($"An error occurred when retrieving the Schedule: {ex.Message}");
            }
        }
        public async Task<Result<Schedule>> CreateSchedule(string fileName, int userTypeId)
        {
            try
            {
                var schedule = new Schedule
                {
                    ImagePath = $"/uploads/{fileName}",
                    UserTypeId = userTypeId
                };
                await schoolHubContext.AddAsync(schedule);
                await schoolHubContext.SaveChangesAsync();
                return Result<Schedule>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<Schedule>.Failure($"An error occurred when creating Schedule: {ex.Message}");
            }
        }
        public async Task<Result<Schedule>> UpdateSchedule(int scheduleId, string fileName)
        {
            try
            {
                var schedule = await schoolHubContext.Schedules.FirstOrDefaultAsync(x => x.Id == scheduleId);
                if (schedule == null)
                {
                    return Result<Schedule>.Failure("Schedule is not found");
                }

                schedule.UserTypeId = schedule.UserTypeId;
                schedule.ImagePath = fileName;
                schedule.ImagePath = $"/uploads/{fileName}";

                await schoolHubContext.SaveChangesAsync();
                return Result<Schedule>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<Schedule>.Failure($"An error occurred when Update Schedule: {ex.Message}");
            }
        }
        public async Task<Result<bool>> DeleteSchedule(int scheduleId)
        {
            try
            {
                var schedule = await schoolHubContext.Schedules.FirstOrDefaultAsync(x => x.Id == scheduleId);
                if (schedule == null)
                {
                    return Result<bool>.Failure("Schedule not found");
                }

                schoolHubContext.Schedules.Remove(schedule);
                await schoolHubContext.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"An error occurred when deleting the Schedule: {ex.Message}");
            }
        }

        //for Other users
        public async Task<Result<Schedule>> GetUserTypeSchedule(int usertypeId)
        {
            try
            {
                var schedule = await schoolHubContext.Schedules
                    .Include(s => s.UserType)
                    .FirstOrDefaultAsync(x => x.UserTypeId == usertypeId);
                if (schedule == null)
                {
                    return Result<Schedule>.Failure("Schedule not found");
                }
                return Result<Schedule>.Success(schedule);
            }
            catch (Exception ex)
            {
                return Result<Schedule>.Failure($"An error occurred when retrieving the Schedule: {ex.Message}");
            }
        }
    }
}
