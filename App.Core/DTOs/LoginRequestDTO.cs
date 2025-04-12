using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "User Id is required")]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}
