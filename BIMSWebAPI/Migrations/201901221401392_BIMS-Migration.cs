namespace BIMSWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BIMSMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(unicode: false),
                        MiddleName = c.String(unicode: false),
                        LastName = c.String(unicode: false),
                        Username = c.String(unicode: false),
                        Password = c.String(unicode: false),
                        Role = c.String(unicode: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
