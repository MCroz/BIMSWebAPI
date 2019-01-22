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
                        Surname = c.String(unicode: false),
                        Username = c.String(unicode: false),
                        Password = c.String(unicode: false),
                        Role = c.String(unicode: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.String(unicode: false),
                        ModifiedBy = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.ID);
            
            DropTable("dbo.Products");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductName = c.String(unicode: false),
                        Price = c.Double(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductId);
            
            DropTable("dbo.Users");
        }
    }
}
