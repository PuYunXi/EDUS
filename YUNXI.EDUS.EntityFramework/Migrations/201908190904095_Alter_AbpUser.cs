namespace YUNXI.EDUS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Alter_AbpUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbpUsers", "ProfilePictureId", c => c.Guid());
            AddColumn("dbo.AbpUsers", "ShouldChangePasswordOnNextLogin", c => c.Boolean(nullable: false));
            AddColumn("dbo.AbpUsers", "WeChatId", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbpUsers", "WeChatId");
            DropColumn("dbo.AbpUsers", "ShouldChangePasswordOnNextLogin");
            DropColumn("dbo.AbpUsers", "ProfilePictureId");
        }
    }
}
