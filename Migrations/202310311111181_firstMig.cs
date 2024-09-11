namespace POEPart2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class firstMig : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        ModuleId = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Credits = c.Int(nullable: false),
                        ClassHoursPerWeek = c.Int(nullable: false),
                        SelfStudyHoursPerWeek = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        SemesterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ModuleId)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.Semesters",
                c => new
                    {
                        SemesterId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        NumberOfWeeks = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SemesterId)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        Email = c.String(),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.ModuleStudyRecords",
                c => new
                    {
                        RecordId = c.Int(nullable: false, identity: true),
                        ModuleCode = c.String(),
                        Date = c.DateTime(nullable: false),
                        HoursWorked = c.Int(nullable: false),
                        HoursLeft = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RecordId)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .ForeignKey("dbo.Modules", t => t.UserId, cascadeDelete: false)
                .Index(t => t.ModuleId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleStudyRecords", "UserId", "dbo.Modules");
            DropForeignKey("dbo.ModuleStudyRecords", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Modules", "UserId", "dbo.Users");
            DropForeignKey("dbo.Modules", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.Semesters", "UserId", "dbo.Users");
            DropIndex("dbo.ModuleStudyRecords", new[] { "UserId" });
            DropIndex("dbo.ModuleStudyRecords", new[] { "ModuleId" });
            DropIndex("dbo.Semesters", new[] { "UserId" });
            DropIndex("dbo.Modules", new[] { "SemesterId" });
            DropIndex("dbo.Modules", new[] { "UserId" });
            DropTable("dbo.ModuleStudyRecords");
            DropTable("dbo.Users");
            DropTable("dbo.Semesters");
            DropTable("dbo.Modules");
        }
    }
}
