using POEPart2;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POEPart2
{
    public class Module
    {
        [Key]
        [Required]
        public int ModuleId { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Credits { get; set; }
        [Required]
        public int ClassHoursPerWeek { get; set; }
        [Required]
        public int SelfStudyHoursPerWeek { get; set; }

        //Foreign Keys
        [ForeignKey(nameof(Users))]
        public int UserId { get; set; }
        public Users Users { get; set; }

        [ForeignKey(nameof(Semester))]
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }

    }
}
