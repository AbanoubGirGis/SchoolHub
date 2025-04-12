using App.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.SchedulesDTOs
{
    public class SchedulesDTO
    {
        public List<Schedule> Schedules { get; set; } = [];
        public int Count { get; set; }
    }
}
