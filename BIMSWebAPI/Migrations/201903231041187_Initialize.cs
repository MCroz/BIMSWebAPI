namespace BIMSWebAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initialize : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accuseds",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(unicode: false),
                        MiddleName = c.String(unicode: false),
                        LastName = c.String(unicode: false),
                        Gender = c.String(unicode: false),
                        Address = c.String(unicode: false),
                        BirthDate = c.DateTime(nullable: false, storeType: "date"),
                        ResidentID = c.Int(),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Residents", t => t.ResidentID)
                .Index(t => t.ResidentID);
            
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
                        isRemoved = c.Int(nullable: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.BarangayClearanceTransactions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ResidentID = c.Int(nullable: false),
                        FullAddress = c.String(unicode: false),
                        FullName = c.String(unicode: false),
                        Image = c.String(unicode: false),
                        Purpose = c.String(unicode: false),
                        ControlNo = c.String(unicode: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Residents", t => t.ResidentID, cascadeDelete: true)
                .Index(t => t.ResidentID);
            
            CreateTable(
                "dbo.Blotters",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ComplainantID = c.Int(nullable: false),
                        AccusedID = c.Int(nullable: false),
                        TypeOfIncident = c.String(unicode: false),
                        IncidentDateTime = c.DateTime(nullable: false, precision: 0),
                        AddressNo = c.String(unicode: false),
                        AddressSt = c.String(unicode: false),
                        AddressZone = c.String(unicode: false),
                        NarrativeReport = c.String(unicode: false),
                        Status = c.String(unicode: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Accuseds", t => t.AccusedID, cascadeDelete: true)
                .ForeignKey("dbo.Complainants", t => t.ComplainantID, cascadeDelete: true)
                .Index(t => t.AccusedID)
                .Index(t => t.ComplainantID);
            
            CreateTable(
                "dbo.Complainants",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(unicode: false),
                        MiddleName = c.String(unicode: false),
                        LastName = c.String(unicode: false),
                        Gender = c.String(unicode: false),
                        Address = c.String(unicode: false),
                        BirthDate = c.DateTime(nullable: false, storeType: "date"),
                        ResidentID = c.Int(),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Residents", t => t.ResidentID)
                .Index(t => t.ResidentID);
            
            CreateTable(
                "dbo.BusinessClearances",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OwnerFullName = c.String(unicode: false),
                        OwnerAddress = c.String(unicode: false),
                        OwnerContactNo = c.String(unicode: false),
                        BusinessName = c.String(unicode: false),
                        BusinessAddress = c.String(unicode: false),
                        FloorArea = c.String(unicode: false),
                        DTI_SEC_RegNo = c.String(unicode: false),
                        BusinessContactNo = c.String(unicode: false),
                        KindOfBusiness = c.String(unicode: false),
                        Capitalization = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BusinessID = c.Int(),
                        ControlNo = c.String(unicode: false),
                        isRemoved = c.Int(nullable: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Businesses", t => t.BusinessID)
                .Index(t => t.BusinessID);
            
            CreateTable(
                "dbo.Businesses",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BusinessName = c.String(unicode: false),
                        BusinessAddress = c.String(unicode: false),
                        FloorArea = c.String(unicode: false),
                        DTI_SEC_RegNo = c.String(unicode: false),
                        BusinessContactNo = c.String(unicode: false),
                        KindOfBusiness = c.String(unicode: false),
                        Capitalization = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OwnerID = c.Int(),
                        isRemoved = c.Int(nullable: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Owners", t => t.OwnerID)
                .Index(t => t.OwnerID);
            
            CreateTable(
                "dbo.Owners",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(unicode: false),
                        MiddleName = c.String(unicode: false),
                        LastName = c.String(unicode: false),
                        Address = c.String(unicode: false),
                        ContactNo = c.String(unicode: false),
                        Image = c.String(unicode: false),
                        isRemoved = c.Int(nullable: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DispenseTransactions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ResidentID = c.Int(nullable: false),
                        Prescriptive = c.Int(nullable: false),
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
                        Days = c.Int(),
                        Intakes = c.Int(),
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
                "dbo.IndigencyTransactions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ResidentID = c.Int(nullable: false),
                        FullAddress = c.String(unicode: false),
                        FullName = c.String(unicode: false),
                        Image = c.String(unicode: false),
                        Purpose = c.String(unicode: false),
                        ControlNo = c.String(unicode: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Residents", t => t.ResidentID, cascadeDelete: true)
                .Index(t => t.ResidentID);
            
            CreateTable(
                "dbo.SecretQuestions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Question = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SystemLogs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LogAction = c.String(unicode: false),
                        LogType = c.String(unicode: false),
                        LogTime = c.DateTime(nullable: false, precision: 0),
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
                        SecretQuestion1ID = c.Int(),
                        SecretQuestion2ID = c.Int(),
                        SecretAnswer1 = c.String(unicode: false),
                        SecretAnswer2 = c.String(unicode: false),
                        Attempt = c.Int(nullable: false),
                        isRemoved = c.Int(nullable: false),
                        DateCreated = c.DateTime(precision: 0),
                        DateModified = c.DateTime(precision: 0),
                        CreatedBy = c.Int(nullable: false),
                        ModifiedBy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SecretQuestions", t => t.SecretQuestion1ID)
                .ForeignKey("dbo.SecretQuestions", t => t.SecretQuestion2ID)
                .Index(t => t.SecretQuestion1ID)
                .Index(t => t.SecretQuestion2ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "SecretQuestion2ID", "dbo.SecretQuestions");
            DropForeignKey("dbo.Users", "SecretQuestion1ID", "dbo.SecretQuestions");
            DropForeignKey("dbo.IndigencyTransactions", "ResidentID", "dbo.Residents");
            DropForeignKey("dbo.DispenseTransactions", "ResidentID", "dbo.Residents");
            DropForeignKey("dbo.Stocks", "MedicineID", "dbo.Medicines");
            DropForeignKey("dbo.InventoryMovements", "StockID", "dbo.Stocks");
            DropForeignKey("dbo.InventoryMovements", "DispenseTransactionID", "dbo.DispenseTransactions");
            DropForeignKey("dbo.Businesses", "OwnerID", "dbo.Owners");
            DropForeignKey("dbo.BusinessClearances", "BusinessID", "dbo.Businesses");
            DropForeignKey("dbo.Blotters", "ComplainantID", "dbo.Complainants");
            DropForeignKey("dbo.Complainants", "ResidentID", "dbo.Residents");
            DropForeignKey("dbo.Blotters", "AccusedID", "dbo.Accuseds");
            DropForeignKey("dbo.BarangayClearanceTransactions", "ResidentID", "dbo.Residents");
            DropForeignKey("dbo.Accuseds", "ResidentID", "dbo.Residents");
            DropIndex("dbo.Users", new[] { "SecretQuestion2ID" });
            DropIndex("dbo.Users", new[] { "SecretQuestion1ID" });
            DropIndex("dbo.IndigencyTransactions", new[] { "ResidentID" });
            DropIndex("dbo.DispenseTransactions", new[] { "ResidentID" });
            DropIndex("dbo.Stocks", new[] { "MedicineID" });
            DropIndex("dbo.InventoryMovements", new[] { "StockID" });
            DropIndex("dbo.InventoryMovements", new[] { "DispenseTransactionID" });
            DropIndex("dbo.Businesses", new[] { "OwnerID" });
            DropIndex("dbo.BusinessClearances", new[] { "BusinessID" });
            DropIndex("dbo.Blotters", new[] { "ComplainantID" });
            DropIndex("dbo.Complainants", new[] { "ResidentID" });
            DropIndex("dbo.Blotters", new[] { "AccusedID" });
            DropIndex("dbo.BarangayClearanceTransactions", new[] { "ResidentID" });
            DropIndex("dbo.Accuseds", new[] { "ResidentID" });
            DropTable("dbo.Users");
            DropTable("dbo.SystemLogs");
            DropTable("dbo.SecretQuestions");
            DropTable("dbo.IndigencyTransactions");
            DropTable("dbo.Medicines");
            DropTable("dbo.Stocks");
            DropTable("dbo.InventoryMovements");
            DropTable("dbo.DispenseTransactions");
            DropTable("dbo.Owners");
            DropTable("dbo.Businesses");
            DropTable("dbo.BusinessClearances");
            DropTable("dbo.Complainants");
            DropTable("dbo.Blotters");
            DropTable("dbo.BarangayClearanceTransactions");
            DropTable("dbo.Residents");
            DropTable("dbo.Accuseds");
        }
    }
}
