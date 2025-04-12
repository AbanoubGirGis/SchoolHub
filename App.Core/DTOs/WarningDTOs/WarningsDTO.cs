using App.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.DTOs.UsersDTOs
{
    public class WarningsDTO
    {
        public List<Warning> Warnings { get; set; } = [];
        public int Count { get; set; }
    }
}
