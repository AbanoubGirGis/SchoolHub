using App.Core.DTOs.UsersDTOs;
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

        public async Task<Result<User>> VerifyUserCredentials(string userId, string password)
        {
            var user = await schoolHubContext.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return Result<User>.Failure("User not found"); ;

            if (user.Password != password)
                return Result<User>.Failure("Invalid password");

            return Result<User>.Success(user);
        }
        public async Task<Result<List<User>>> GetUsers()
        {
            try
            {
                var users = await schoolHubContext.Users
                    .Include(u => u.UserType)
                    .ToListAsync();

                return Result<List<User>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<List<User>>.Failure($"Error retrieving users: {ex.Message}");
            }
        }
        public async Task<Result<User>> GetUser(string userId)
        {
            try
            {
                var user = await schoolHubContext.Users
                    .Include(u => u.UserType)
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
        public async Task<Result<User>> CreateUser(UserDTO userDTO)
        {
            try
            {
                var user = new User
                {
                    UserId = userDTO.UserId,
                    FullName = userDTO.FullName,
                    Email = userDTO.Email,
                    Password = userDTO.Password,
                    UserTypeId = userDTO.UserTypeId,
                    CreatedAt = DateTime.Now,
                };
                await schoolHubContext.Users.AddAsync(user);
                await schoolHubContext.SaveChangesAsync();
                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error creating user: {ex.Message}");
            }
        }
        public async Task<Result<User>> UpdateUser(UserDTO userDTO)
        {
            try
            {
                var existingUser = await schoolHubContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == userDTO.UserId);

                if (existingUser == null)
                {
                    return Result<User>.Failure("User not found.");
                }


                existingUser.UserId = userDTO.UserId;
                existingUser.FullName = userDTO.FullName;
                existingUser.Email = userDTO.Email;
                existingUser.Password = userDTO.Password;
                existingUser.UserTypeId = userDTO.UserTypeId;
                existingUser.CreatedAt = DateTime.Now;

                schoolHubContext.Users.Update(existingUser);

                await schoolHubContext.SaveChangesAsync();
                return Result<User>.Success(existingUser);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error Update user: {ex.Message}");
            }
        }
        public async Task<Result<User>> DeleteUser(string userId)
        {
            try
            {
                var user = await schoolHubContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                    return Result<User>.Failure("User not found");

                var subject = await schoolHubContext.Subjects.FirstOrDefaultAsync(u => u.TeacherId == user.Id);
                if (subject != null)
                {
                    subject.TeacherId = null;
                    schoolHubContext.Subjects.Update(subject);
                }

                var grade = await schoolHubContext.Grades.FirstOrDefaultAsync(u => u.TeacherId == user.Id);
                if (grade != null)
                {
                    grade.TeacherId = null;
                    schoolHubContext.Grades.Update(grade);
                }

                schoolHubContext.Users.Remove(user);
                await schoolHubContext.SaveChangesAsync();

                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error deleting user: {ex.Message}");
            }
        }

        public async Task<Result<List<User>>> GetTeachers()
        {
            try
            {
                var users = await schoolHubContext.Users.Where(x => x.UserTypeId == 3)
                    .Include(u => u.UserType)
                    .ToListAsync();

                return Result<List<User>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<List<User>>.Failure($"Error retrieving users: {ex.Message}");
            }
        }
        public async Task<Result<List<User>>> GetStudents()
        {
            try
            {
                var users = await schoolHubContext.Users.Where(x => x.UserTypeId == 2)
                    .Include(u => u.UserType)
                    .ToListAsync();

                return Result<List<User>>.Success(users);
            }
            catch (Exception ex)
            {
                return Result<List<User>>.Failure($"Error retrieving users: {ex.Message}");
            }
        }

        public async Task<Result<User>> ForgetPassword(string userId)
        {
            try
            {
                var user = await schoolHubContext.Users
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return Result<User>.Failure("User is not Found");
                }

                user.Password = GeneratePassword();
                schoolHubContext.Users.Update(user);
                await schoolHubContext.SaveChangesAsync();

                return Result<User>.Success(user);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error Update user new password: {ex.Message}");
            }
        }
        private string GeneratePassword(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }

            return password.ToString();
        }
    }
}