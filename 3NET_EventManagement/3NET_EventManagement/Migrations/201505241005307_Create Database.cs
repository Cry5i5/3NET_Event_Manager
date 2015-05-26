namespace _3NET_EventManagement.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventName = c.String(),
                        Address = c.String(),
                        Date = c.DateTime(nullable: false),
                        Summary = c.String(),
                        CreatorId = c.Int(nullable: false),
                        StatusId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatorId, cascadeDelete: true)
                .ForeignKey("dbo.Status", t => t.StatusId, cascadeDelete: true)
                .ForeignKey("dbo.EventTypes", t => t.TypeId, cascadeDelete: true)
                .Index(t => t.CreatorId)
                .Index(t => t.StatusId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.Status",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StatusName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Contributions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        Description = c.String(),
                        EventId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        TypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.ContributionTypes", t => t.TypeId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.UserId)
                .Index(t => t.TypeId);
            
            CreateTable(
                "dbo.ContributionTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContributionTypeName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Invitations",
                c => new
                    {
                        Event_id = c.Int(nullable: false),
                        User_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Event_id, t.User_id })
                .ForeignKey("dbo.Events", t => t.Event_id, cascadeDelete: false)
                .ForeignKey("dbo.Users", t => t.User_id, cascadeDelete: false)
                .Index(t => t.Event_id)
                .Index(t => t.User_id);
            
            CreateTable(
                "dbo.UserUsers",
                c => new
                    {
                        User_Id = c.Int(nullable: false),
                        User_Id1 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_Id, t.User_Id1 })
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.Users", t => t.User_Id1)
                .Index(t => t.User_Id)
                .Index(t => t.User_Id1);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserUsers", new[] { "User_Id1" });
            DropIndex("dbo.UserUsers", new[] { "User_Id" });
            DropIndex("dbo.Invitations", new[] { "User_id" });
            DropIndex("dbo.Invitations", new[] { "Event_id" });
            DropIndex("dbo.Contributions", new[] { "TypeId" });
            DropIndex("dbo.Contributions", new[] { "UserId" });
            DropIndex("dbo.Contributions", new[] { "EventId" });
            DropIndex("dbo.Events", new[] { "TypeId" });
            DropIndex("dbo.Events", new[] { "StatusId" });
            DropIndex("dbo.Events", new[] { "CreatorId" });
            DropForeignKey("dbo.UserUsers", "User_Id1", "dbo.Users");
            DropForeignKey("dbo.UserUsers", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Invitations", "User_id", "dbo.Users");
            DropForeignKey("dbo.Invitations", "Event_id", "dbo.Events");
            DropForeignKey("dbo.Contributions", "TypeId", "dbo.ContributionTypes");
            DropForeignKey("dbo.Contributions", "UserId", "dbo.Users");
            DropForeignKey("dbo.Contributions", "EventId", "dbo.Events");
            DropForeignKey("dbo.Events", "TypeId", "dbo.EventTypes");
            DropForeignKey("dbo.Events", "StatusId", "dbo.Status");
            DropForeignKey("dbo.Events", "CreatorId", "dbo.Users");
            DropTable("dbo.UserUsers");
            DropTable("dbo.Invitations");
            DropTable("dbo.ContributionTypes");
            DropTable("dbo.Contributions");
            DropTable("dbo.EventTypes");
            DropTable("dbo.Status");
            DropTable("dbo.Events");
            DropTable("dbo.Users");
        }
    }
}
