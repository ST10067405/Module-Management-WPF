using POEPart2;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace POEPart2
{
    public class ModuleStudyRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RecordId { get; set; }
        public string ModuleCode { get; set; }
        public DateTime Date { get; set; }
        public int HoursWorked { get; set; }
        public int HoursLeft { get; set; }

        //Foreign Keys
        [ForeignKey(nameof(Module))]
        public int ModuleId { get; set; }
        public Module Module { get; set; }

        [ForeignKey(nameof(Users))]
        public int UserId { get; set; }
        public Module Users { get; set; }
    }
}
