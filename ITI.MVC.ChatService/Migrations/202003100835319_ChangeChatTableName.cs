namespace ITI.MVC.ChatService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeChatTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Chat", newName: "ChatMessage");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ChatMessage", newName: "Chat");
        }
    }
}
