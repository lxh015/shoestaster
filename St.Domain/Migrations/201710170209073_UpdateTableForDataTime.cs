namespace St.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateTableForDataTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Nor_Ads", "AddDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Nor_Ads", "UpdateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Nor_Images", "AddDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Nor_Images", "UpdateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Nor_NewsMain", "AddDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Nor_NewsMain", "UpdateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Pro_Products", "AddDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Pro_Products", "UpdateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sys_SUser", "AddDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Sys_SUser", "UpdateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sys_SUser", "UpdateTime");
            DropColumn("dbo.Sys_SUser", "AddDateTime");
            DropColumn("dbo.Pro_Products", "UpdateTime");
            DropColumn("dbo.Pro_Products", "AddDateTime");
            DropColumn("dbo.Nor_NewsMain", "UpdateTime");
            DropColumn("dbo.Nor_NewsMain", "AddDateTime");
            DropColumn("dbo.Nor_Images", "UpdateTime");
            DropColumn("dbo.Nor_Images", "AddDateTime");
            DropColumn("dbo.Nor_Ads", "UpdateTime");
            DropColumn("dbo.Nor_Ads", "AddDateTime");
        }
    }
}
