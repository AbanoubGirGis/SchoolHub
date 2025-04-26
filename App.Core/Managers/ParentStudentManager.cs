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
    public class ParentStudentManager
    {
        private readonly SchoolHubContext schoolHubContext;

        public ParentStudentManager(SchoolHubContext schoolHubContext)
        {
            this.schoolHubContext = schoolHubContext;
        }

        public async Task<Result<List<ParentStudent>>> GetAllStudentsParents()
        {
            try
            {
                var ParentStudents = await schoolHubContext.ParentStudents
                    .Include(p => p.Parent)
                    .Include(p => p.Student)
                    .ToListAsync();

                if (ParentStudents == null)
                    return Result<List<ParentStudent>>.Failure("No students found linked to parents");


                return Result<List<ParentStudent>>.Success(ParentStudents);
            }
            catch (Exception ex)
            {
                return Result<List<ParentStudent>>.Failure($"Error retrieving students for parents: {ex.Message}");
            }
        }

        public async Task<Result<User>> GetStudentByParentId(string parentUserId)
        {
            try
            {
                var parent = await schoolHubContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == parentUserId && u.UserTypeId == 4);

                if (parent == null)
                    return Result<User>.Failure("Parent user not found or user is not a parent");

                var studentParent = await schoolHubContext.ParentStudents
                    .FirstOrDefaultAsync(ps => ps.ParentId == parent.Id);

                if (studentParent == null)
                    return Result<User>.Failure("No students found linked to this parent");

                var student = await schoolHubContext.Users
                   .FirstOrDefaultAsync(u => u.Id == studentParent.StudentId);

                if (student == null)
                    return Result<User>.Failure("No students found linked to this parent");

                return Result<User>.Success(student);
            }
            catch (Exception ex)
            {
                return Result<User>.Failure($"Error retrieving student for parent: {ex.Message}");
            }
        }

        public async Task<Result<ParentStudent>> AssignStudentToParent(string parentUserId, string studentUserId)
        {
            try
            {
                // Verify parent exists and is a parent type
                var parent = await schoolHubContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == parentUserId && u.UserTypeId == 4);

                if (parent == null)
                    return Result<ParentStudent>.Failure("Parent not found or user is not a parent");

                // Verify student exists and is a student type
                var student = await schoolHubContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == studentUserId && u.UserTypeId == 2);

                if (student == null)
                    return Result<ParentStudent>.Failure("Student not found or user is not a student");

                var parentStudent = new ParentStudent
                {
                    ParentId = parent.Id,
                    StudentId = student.Id
                };

                await schoolHubContext.ParentStudents.AddAsync(parentStudent);
                await schoolHubContext.SaveChangesAsync();

                return Result<ParentStudent>.Success(parentStudent);
            }
            catch (Exception ex)
            {
                return Result<ParentStudent>.Failure($"Error assigning student to parent: {ex.Message}");
            }
        }

        public async Task<Result<ParentStudent>> UpdateStudentToParent(string parentUserId, string studentUserId)
        {
            try
            {
                // Verify parent exists and is a parent type
                var parent = await schoolHubContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == parentUserId && u.UserTypeId == 4);

                if (parent == null)
                    return Result<ParentStudent>.Failure("Parent not found or user is not a parent");

                // Verify student exists and is a student type
                var student = await schoolHubContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == studentUserId && u.UserTypeId == 2);

                if (student == null)
                    return Result<ParentStudent>.Failure("Student not found or user is not a student");



                // Check if relationship already exists
                var existingParentStudent = await schoolHubContext.ParentStudents
                    .FirstOrDefaultAsync(ps => ps.ParentId == parent.Id);

                if (existingParentStudent == null)
                    return Result<ParentStudent>.Failure("This student is not assigned to this parent");

                existingParentStudent.ParentId = parent.Id;
                existingParentStudent.StudentId = student.Id;

                schoolHubContext.ParentStudents.Update(existingParentStudent);
                await schoolHubContext.SaveChangesAsync();

                return Result<ParentStudent>.Success(existingParentStudent);
            }
            catch (Exception ex)
            {
                return Result<ParentStudent>.Failure($"Error update student to parent: {ex.Message}");
            }
        }

        public async Task<Result<ParentStudent>> DeleteStudentByParentId(string parentUserId)
        {
            try
            {
                var parent = await schoolHubContext.Users
                    .FirstOrDefaultAsync(u => u.UserId == parentUserId && u.UserTypeId == 4);

                if (parent == null)
                    return Result<ParentStudent>.Failure("Parent user not found or user is not a parent");

                var ParentStudent = await schoolHubContext.ParentStudents
                    .FirstOrDefaultAsync(ps => ps.ParentId == parent.Id);

                if (ParentStudent == null)
                    return Result<ParentStudent>.Failure("No students found linked to this parent");

                schoolHubContext.ParentStudents.Remove(ParentStudent);
                await schoolHubContext.SaveChangesAsync();
                return Result<ParentStudent>.Success(ParentStudent);
            }
            catch (Exception ex)
            {
                return Result<ParentStudent>.Failure($"Error deleting students for parent: {ex.Message}");
            }
        }
    }
}