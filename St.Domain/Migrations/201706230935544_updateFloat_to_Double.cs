namespace St.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateFloat_to_Double : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Pro_Products", "minPrice", c => c.Double());
            AlterColumn("dbo.Pro_Products", "maxPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Pro_Products", "maxPrice", c => c.Single(nullable: false));
            AlterColumn("dbo.Pro_Products", "minPrice", c => c.Single());
        }
    }
}
