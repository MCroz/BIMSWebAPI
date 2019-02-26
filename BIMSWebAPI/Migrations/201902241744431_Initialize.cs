namespace BIMSWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Residents", "AddressNo", c => c.String(unicode: false));
            AddColumn("dbo.Residents", "AddressSt", c => c.String(unicode: false));
            DropColumn("dbo.Residents", "Address");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Residents", "Address", c => c.String(unicode: false));
            DropColumn("dbo.Residents", "AddressSt");
            DropColumn("dbo.Residents", "AddressNo");
        }
    }
}
