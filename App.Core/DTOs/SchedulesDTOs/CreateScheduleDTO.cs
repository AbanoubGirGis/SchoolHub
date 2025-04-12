using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.SchedulesDTOs
{
    public class CreateScheduleDTO
    {
        public int UserTypeId { get; set; }
        public IFormFile? FormFile { get; set; }
    }
}
