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
    public class UserManager
    {
        private readonly SchoolHubContext schoolHubContext;

        public UserManager(SchoolHubContext schoolHubContext)
        {
            this.schoolHubContext = schoolHubContext;
        }

        public async Task<Result<User>> CheckUser(int id, string password)
        {
            var user = await schoolHubContext.Users
                .Include(u => u.UserTypeNavigation)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return Result<User>.Failure("User not found"); ;

            if (user.Password != password)
                return Result<User>.Failure("Invalid password");

            return Result<User>.Success(user);
        }

        public async Task<Result<User>> UpdateUserOtp(User user)
        {
            try
            {
                var existingUser = await schoolHubContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == user.UserId || u.Id == user.Id);

                if (existingUser != null)
                {
                    existingUser.OtptimeOut = DateTime.Now;

                    schoolHubContext.Users.Update(existingUser);
                    await schoolHubContext.SaveChangesAsync();

                    return Result<User>.Success(user);
                }
                else
                {
                    return Result<User>.Failure($"User is not found");
                }
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Result<User>> CheckUserOTP(int userId, int otp)
        {
            try
            {
                var existingUser = await schoolHubContext.Users
                    .Include(u => u.UserTypeNavigation)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (existingUser == null)
                {
                    return Result<User>.Failure("User not found.");
                }

                var currentTime = DateTime.Now;
                var otpTime = existingUser.OtptimeOut;
                var otpValidDuration = TimeSpan.FromMinutes(10);

                if (existingUser.Otp == otp)
                {
                    if (currentTime - otpTime <= otpValidDuration)
                    {
                        return Result<User>.Success(existingUser);
                    }
                    else
                    {
                        return Result<User>.Failure("OTP has expired.");
                    }
                }
                else
                {
                    return Result<User>.Failure("Invalid OTP.");
                }
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Result<User>> GetUserById(int userId)
        {
            try
            {
                var user = await schoolHubContext.Users
                    .Include(u => u.UserTypeNavigation)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    return Result<User>.Failure("User not found");

                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error retrieving user: {ex.Message}");
            }
        }
        public async Task<Result<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await schoolHubContext.Users
                    .Include(u => u.UserTypeNavigation)
                    .ToListAsync();

                return Result<List<User>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<List<User>>.Failure($"Error retrieving users: {ex.Message}");
            }
        }
        public async Task<Result<User>> CreateUser(User user)
        {
            try
            {
                await schoolHubContext.Users.AddAsync(user);
                await schoolHubContext.SaveChangesAsync();
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error creating user: {ex.Message}");
            }
        }
        public async Task<Result<User>> UpdateUser(User user)
        {
            try
            {
                var existingUser = await schoolHubContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == user.UserId || u.Id == user.Id);

                if (existingUser == null)
                {
                    return Result<User>.Failure("User not found.");
                }

                existingUser.Password = user.Password;
                existingUser.Email = user.Email;
                existingUser.Otp = user.Otp;
                existingUser.OtptimeOut = user.OtptimeOut;
                existingUser.UserType = user.UserType;

                schoolHubContext.Users.Update(existingUser);
                await schoolHubContext.SaveChangesAsync();

                return Result<User>.Success(existingUser);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"An error occurred while updating: {ex.Message}");
            }
        }
        public async Task<Result<bool>> DeleteUser(int userId)
        {
            try
            {
                var user = await schoolHubContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    return Result<bool>.Failure("User not found");

                schoolHubContext.Users.Remove(user);
                await schoolHubContext.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting user: {ex.Message}");
            }
        }
    }
}