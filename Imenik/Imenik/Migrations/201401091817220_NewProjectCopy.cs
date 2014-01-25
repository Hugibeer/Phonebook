namespace Imenik.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewProjectCopy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        ContactId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        City = c.String(),
                        ImgUri = c.String(),
                        Description = c.String(),
                        OwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ContactId)
                .ForeignKey("dbo.Owners", t => t.OwnerId, cascadeDelete: true)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Phones",
                c => new
                    {
                        PhoneId = c.Int(nullable: false, identity: true),
                        PhoneNumber = c.String(),
                        PhoneType = c.String(),
                        PhoneDescription = c.String(),
                        ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PhoneId)
                .ForeignKey("dbo.Contacts", t => t.ContactId, cascadeDelete: true)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.Owners",
                c => new
                    {
                        OwnerId = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                    })
                .PrimaryKey(t => t.OwnerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Contacts", "OwnerId", "dbo.Owners");
            DropForeignKey("dbo.Phones", "ContactId", "dbo.Contacts");
            DropIndex("dbo.Contacts", new[] { "OwnerId" });
            DropIndex("dbo.Phones", new[] { "ContactId" });
            DropTable("dbo.Owners");
            DropTable("dbo.Phones");
            DropTable("dbo.Contacts");
        }
    }
}
