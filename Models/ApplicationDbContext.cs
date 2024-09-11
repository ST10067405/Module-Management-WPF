using ModulesLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEPart2
{
    public class ApplicationDbContext : DbContext
    {
        // Converting c# classes into SQL tables
        // creating DbSets using applicationDbContext
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ModuleStudyRecord> ModuleStudyRecords { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
