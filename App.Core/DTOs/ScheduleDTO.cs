using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs
{
    public class ScheduleDTO
    {
        public int ScheduleId { get; set; }
        public int UserTypeId { get; set; }
        public string? FilePath { get; set; }
        public IFormFile? FormFile { get; set; }
    }
}
