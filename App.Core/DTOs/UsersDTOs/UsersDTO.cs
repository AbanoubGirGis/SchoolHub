using App.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.UsersDTOs
{
    public class UsersDTO
    {
        public List<User> Users { get; set; } = [];
        public int Count { get; set; }
    }
}
