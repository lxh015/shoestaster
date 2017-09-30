namespace St.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewShowChange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Nor_Images",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Context = c.String(),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Nor_NewsMain",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Stata = c.Int(nullable: false),
                        Title = c.String(),
                        Summary = c.String(),
                        Context = c.String(),
                        IsShow = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Nor_NewsShow",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Summary = c.String(),
                        Title = c.String(),
                        images_ID = c.Int(),
                        MainID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Nor_Images", t => t.images_ID)
                .ForeignKey("dbo.Nor_NewsMain", t => t.MainID, cascadeDelete: true)
                .Index(t => t.images_ID)
                .Index(t => t.MainID);
            
            CreateTable(
                "dbo.Pro_ProductClass",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        isShow = c.Boolean(nullable: false),
                        Stata = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Pro_ProdctClassIntroduction",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        PCID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Pro_ProductClass", t => t.PCID, cascadeDelete: true)
                .Index(t => t.PCID);
            
            CreateTable(
                "dbo.Pro_ProductImages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Alink = c.String(),
                        IID = c.Int(),
                        PID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Nor_Images", t => t.IID)
                .ForeignKey("dbo.Pro_Products", t => t.PID, cascadeDelete: true)
                .Index(t => t.IID)
                .Index(t => t.PID);
            
            CreateTable(
                "dbo.Pro_Products",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        minPrice = c.Single(),
                        maxPrice = c.Single(nullable: false),
                        Introduction = c.String(),
                        Context = c.String(),
                        isShow = c.Boolean(nullable: false),
                        ClassIntroduction = c.String(),
                        Stata = c.Int(nullable: false),
                        PCID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Pro_ProductClass", t => t.PCID)
                .Index(t => t.PCID);
            
            CreateTable(
                "dbo.Sys_SUser",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        NickName = c.String(),
                        PassWord = c.String(nullable: false),
                        isUse = c.Boolean(nullable: false),
                        Level = c.Int(nullable: false),
                        Stata = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Nor_Ads",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LinkUrl = c.String(),
                        Title = c.String(),
                        Area = c.Int(nullable: false),
                        Stata = c.Int(nullable: false),
                        ImageID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Nor_Images", t => t.ImageID)
                .Index(t => t.ImageID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Nor_Ads", "ImageID", "dbo.Nor_Images");
            DropForeignKey("dbo.Pro_ProductImages", "PID", "dbo.Pro_Products");
            DropForeignKey("dbo.Pro_Products", "PCID", "dbo.Pro_ProductClass");
            DropForeignKey("dbo.Pro_ProductImages", "IID", "dbo.Nor_Images");
            DropForeignKey("dbo.Pro_ProdctClassIntroduction", "PCID", "dbo.Pro_ProductClass");
            DropForeignKey("dbo.Nor_NewsShow", "MainID", "dbo.Nor_NewsMain");
            DropForeignKey("dbo.Nor_NewsShow", "images_ID", "dbo.Nor_Images");
            DropIndex("dbo.Nor_Ads", new[] { "ImageID" });
            DropIndex("dbo.Pro_Products", new[] { "PCID" });
            DropIndex("dbo.Pro_ProductImages", new[] { "PID" });
            DropIndex("dbo.Pro_ProductImages", new[] { "IID" });
            DropIndex("dbo.Pro_ProdctClassIntroduction", new[] { "PCID" });
            DropIndex("dbo.Nor_NewsShow", new[] { "MainID" });
            DropIndex("dbo.Nor_NewsShow", new[] { "images_ID" });
            DropTable("dbo.Nor_Ads");
            DropTable("dbo.Sys_SUser");
            DropTable("dbo.Pro_Products");
            DropTable("dbo.Pro_ProductImages");
            DropTable("dbo.Pro_ProdctClassIntroduction");
            DropTable("dbo.Pro_ProductClass");
            DropTable("dbo.Nor_NewsShow");
            DropTable("dbo.Nor_NewsMain");
            DropTable("dbo.Nor_Images");
        }
    }
}
