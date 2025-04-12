using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.SubjectDTOs
{
    public class UpdateSubjectDTO
    {
        public required int SubjectId { get; set; }
        public required string Name { get; set; }
        public required int TeacherId { get; set; }
    }
}
