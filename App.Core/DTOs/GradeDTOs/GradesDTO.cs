using App.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.GradeDTOs
{
    public class GradesDTO
    {
        public List<Grade> Grades { get; set; } = [];
        public int Count { get; set; }
    }
}
