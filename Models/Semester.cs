using POEPart2;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEPart2
{
    public class Semester
    {
        [Key]
        [Required]
        public int SemesterId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int NumberOfWeeks { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }

        //Foreign Keys
        [ForeignKey(nameof(Users))]
        public int UserId { get; set; }
        public Users Users { get; set; }
    }  
}
