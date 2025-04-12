using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.WarningDTOs
{
    public class WarningDTO
    {
        public required int SubjectId { get; set; }
        public required string StudentId { get; set; }
        public required string Reason { get; set; }
    }
}
