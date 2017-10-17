namespace St.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateStateErrorMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Nor_Ads", "FailedReason", c => c.String());
            AddColumn("dbo.Nor_NewsMain", "FailedReason", c => c.String());
            AddColumn("dbo.Pro_ProductClass", "FailedReason", c => c.String());
            AddColumn("dbo.Pro_Products", "FailedReason", c => c.String());
            AddColumn("dbo.Sys_SUser", "FailedReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sys_SUser", "FailedReason");
            DropColumn("dbo.Pro_Products", "FailedReason");
            DropColumn("dbo.Pro_ProductClass", "FailedReason");
            DropColumn("dbo.Nor_NewsMain", "FailedReason");
            DropColumn("dbo.Nor_Ads", "FailedReason");
        }
    }
}
