namespace ITI.MVC.ChatService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessageStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Chat", "MessageStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Chat", "MessageStatus");
        }
    }
}
