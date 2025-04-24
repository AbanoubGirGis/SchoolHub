using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.GradeDTOs
{
    public class UpdateCreateGradeDTO
    {
        public int Id { get; set; }
        public string TeacherId { get; set; }
        public string StudentId { get; set; }
        public int SubjectId { get; set; }
        public string ExamType { get; set; }
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
    }
}
