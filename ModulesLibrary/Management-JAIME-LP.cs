using System;
using System.Collections.Generic;
using System.Reflection;

namespace ModulesLibrary
{
    public class Semester
    {
        public Semester(string name, int numberOfWeeks, DateTime startDate)
        {
            Name = name;
            NumberOfWeeks = numberOfWeeks;
            StartDate = startDate;
        }

        public string Name { get; set; }
        public int NumberOfWeeks { get; set; }
        public DateTime StartDate { get; set; }

        public List<Module> Modules = new List<Module>();
    }

    public class Module
    {
        public Module(string code, string name, int credits, int classHoursPerWeek, int selfStudyHoursPerWeek)
        {
            Code = code;
            Name = name;
            Credits = credits;
            ClassHoursPerWeek = classHoursPerWeek;
            SelfStudyHoursPerWeek = selfStudyHoursPerWeek;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int ClassHoursPerWeek { get; set; }
        public int SelfStudyHoursPerWeek { get; set; }
        public int totalSelfStudyHours { get; set; }
        public List<ModuleStudyRecord> StudiedHoursRecords = new List<ModuleStudyRecord>();

    }

    public class ModuleStudyRecord
    {
        public string ModuleCode { get; set; }
        public DateTime Date { get; set; }
        public int HoursWorked { get; set; }
    }

    public class ModuleManagement
    {
        public static int selfStudyCalc(int credits, int weeks, int classHrs)
        {
            int selfStudyHrs = (credits * 10 / weeks) - classHrs;
            return selfStudyHrs;
        }

        public static int TotalSelfStudyHours(int selfStudyPerWeek, int totalweeks)
        {
            return selfStudyPerWeek;
        }
    }


        

    
}
