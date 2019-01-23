namespace BIMSWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedResidents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Residents",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(unicode: false),
                        MiddleName = c.String(unicode: false),
                        LastName = c.String(unicode: false),
                        Gender = c.String(unicode: false),
                        CivilStatus = c.String(unicode: false),
                        BirthDate = c.DateTime(nullable: false, precision: 0),
                        AddressNo = c.String(unicode: false),
                        AddressSt = c.String(unicode: false),
                        AddressZone = c.String(unicode: false),
                        BirthPlace = c.String(unicode: false),
                        Citizenship = c.String(unicode: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Residents");
        }
    }
}
