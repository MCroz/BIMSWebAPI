namespace BIMSWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BIMSMigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BarangayClearanceTransactions", "FullAddress", c => c.String(unicode: false));
            AddColumn("dbo.BarangayClearanceTransactions", "FullName", c => c.String(unicode: false));
            AddColumn("dbo.BarangayClearanceTransactions", "Image", c => c.String(unicode: false));
            AddColumn("dbo.IndigencyTransactions", "FullAddress", c => c.String(unicode: false));
            AddColumn("dbo.IndigencyTransactions", "FullName", c => c.String(unicode: false));
            AddColumn("dbo.IndigencyTransactions", "Image", c => c.String(unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.IndigencyTransactions", "Image");
            DropColumn("dbo.IndigencyTransactions", "FullName");
            DropColumn("dbo.IndigencyTransactions", "FullAddress");
            DropColumn("dbo.BarangayClearanceTransactions", "Image");
            DropColumn("dbo.BarangayClearanceTransactions", "FullName");
            DropColumn("dbo.BarangayClearanceTransactions", "FullAddress");
        }
    }
}
