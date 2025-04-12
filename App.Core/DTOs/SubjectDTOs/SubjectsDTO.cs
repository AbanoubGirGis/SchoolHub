using App.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.SubjectDTOs
{
    public class SubjectsDTO
    {
        public List<Subject> Subjects { get; set; } = [];
        public int Count { get; set; }
    }
}
