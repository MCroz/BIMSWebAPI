namespace BIMSWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReInitialize : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DispenseTransactions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ResidentID = c.Int(nullable: false),
                        PrescriptionDescription = c.String(unicode: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Residents", t => t.ResidentID, cascadeDelete: true)
                .Index(t => t.ResidentID);
            
            CreateTable(
                "dbo.InventoryMovements",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StockID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        DispenseTransactionID = c.Int(),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DispenseTransactions", t => t.DispenseTransactionID)
                .ForeignKey("dbo.Stocks", t => t.StockID, cascadeDelete: true)
                .Index(t => t.DispenseTransactionID)
                .Index(t => t.StockID);
            
            CreateTable(
                "dbo.Stocks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MedicineID = c.Int(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false, storeType: "date"),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Medicines", t => t.MedicineID, cascadeDelete: true)
                .Index(t => t.MedicineID);
            
            CreateTable(
                "dbo.Medicines",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MedicineName = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
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
                        BirthDate = c.DateTime(nullable: false, storeType: "date"),
                        AddressNo = c.String(unicode: false),
                        AddressSt = c.String(unicode: false),
                        AddressZone = c.String(unicode: false),
                        BirthPlace = c.String(unicode: false),
                        Citizenship = c.String(unicode: false),
                        Image = c.String(unicode: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
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
            DropForeignKey("dbo.DispenseTransactions", "ResidentID", "dbo.Residents");
            DropForeignKey("dbo.Stocks", "MedicineID", "dbo.Medicines");
            DropForeignKey("dbo.InventoryMovements", "StockID", "dbo.Stocks");
            DropForeignKey("dbo.InventoryMovements", "DispenseTransactionID", "dbo.DispenseTransactions");
            DropIndex("dbo.DispenseTransactions", new[] { "ResidentID" });
            DropIndex("dbo.Stocks", new[] { "MedicineID" });
            DropIndex("dbo.InventoryMovements", new[] { "StockID" });
            DropIndex("dbo.InventoryMovements", new[] { "DispenseTransactionID" });
            DropTable("dbo.Users");
            DropTable("dbo.Residents");
            DropTable("dbo.Medicines");
            DropTable("dbo.Stocks");
            DropTable("dbo.InventoryMovements");
            DropTable("dbo.DispenseTransactions");
        }
    }
}
