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
    public class OtpManager
    {
        private readonly SchoolHubContext schoolHubContext;

        public OtpManager(SchoolHubContext schoolHubContext)
        {
            this.schoolHubContext = schoolHubContext;
        }

        public async Task<Result<Otp>> CreateOrUpdateUserOtp(int userId)
        {
            try
            {
                // Check if OTP already exists for the user
                var otp = await schoolHubContext.Otps.FirstOrDefaultAsync(o => o.UserId == userId);

                if (otp != null)
                {
                    // If OTP exists, update it
                    otp.Code = GenerateOtp();
                    otp.CreatedAt = DateTime.Now;
                    await schoolHubContext.SaveChangesAsync();
                    return Result<Otp>.Success(otp);
                }
                else
                {
                    // If OTP doesn't exist, create a new one
                    var newOtp = new Otp
                    {
                        UserId = userId,
                        Code = GenerateOtp(),
                        CreatedAt = DateTime.Now
                    };
                    await schoolHubContext.Otps.AddAsync(newOtp);
                    await schoolHubContext.SaveChangesAsync();
                    return Result<Otp>.Success(newOtp);
                }
            }
            catch (Exception ex)
            {
                return Result<Otp>.Failure($"An error occurred when creating or updating User OTP: {ex.Message}");
            }
        }

        public async Task<Result<Otp>> CheckUserOTP(string userId, int otp)
        {
            try
            {
                var existinUser = await schoolHubContext.Users.FirstOrDefaultAsync(user => user.UserId == userId);
                if (existinUser == null)
                {
                    return Result<Otp>.Failure("User not found.");
                }

                var existingOtp = await schoolHubContext.Otps
                    .Include(u => u.User)
                            .ThenInclude(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.UserId == existinUser.Id);

                if (existingOtp == null)
                {
                    return Result<Otp>.Failure("User OTP not found.");
                }

                var currentTime = DateTime.Now;
                var otpTime = existingOtp.CreatedAt;
                var otpValidDuration = TimeSpan.FromMinutes(1);

                if (existingOtp.Code == otp)
                {
                    if (currentTime - otpTime <= otpValidDuration)
                    {
                        return Result<Otp>.Success(existingOtp);
                    }
                    else
                    {
                        return Result<Otp>.Failure("OTP has expired.");
                    }
                }
                else
                {
                    return Result<Otp>.Failure("Invalid OTP.");
                }
            }
            catch (Exception ex)
            {
                return Result<Otp>.Failure($"An error occurred Check User OTP: {ex.Message}");
            }
        }

        // Method to generate a random 6-digit OTP (One Time Password)
        private int GenerateOtp()
        {
            // Create a new instance of the Random class to generate random numbers
            // The Next method generates a random integer between the specified range (100000 to 999999)
            return new Random().Next(100000, 999999);  // This generates a random number with 6 digits
        }
    }
}
