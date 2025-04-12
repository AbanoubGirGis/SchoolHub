using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.UsersDTOs
{
    public class UserDTO
    {
        public required string UserId { get; set; }

        public required string FullName { get; set; }

        public required string Email { get; set; }

        public required string Password { get; set; }

        public required int? UserTypeId { get; set; }
    }
}
