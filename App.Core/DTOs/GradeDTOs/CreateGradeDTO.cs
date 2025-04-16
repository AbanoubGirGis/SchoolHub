using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.GradeDTOs
{
    public class CreateGradeDTO
    {
        public int TeacherId { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public string ExamType { get; set; }
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
