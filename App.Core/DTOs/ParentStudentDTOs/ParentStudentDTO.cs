using App.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.ParentStudentDTOs
{
    public class ParentStudentDTO
    {
        public List<ParentStudent> ParentStudents { get; set; } = [];
        public int Count { get; set; }
    }
}
