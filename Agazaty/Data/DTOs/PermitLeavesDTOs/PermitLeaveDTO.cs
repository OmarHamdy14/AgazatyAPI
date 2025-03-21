﻿using Agazaty.Models;
using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.PermitLeavesDTOs
{
    public class PermitLeaveDTO
    {
        public int Id { get; set; }
        public double Hours { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
    }
}
