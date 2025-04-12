using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.SchedulesDTOs
{
    public class UpdateScheduleDTO
    {
        public int ScheduleId { get; set; }
        public IFormFile? FormFile { get; set; }
    }
}
