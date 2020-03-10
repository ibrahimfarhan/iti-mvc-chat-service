namespace ITI.MVC.ChatService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdForChatMessage : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChatMessage", "SenderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChatMessage", "ReceiverId", "dbo.AspNetUsers");
            DropIndex("dbo.ChatMessage", new[] { "SenderId" });
            DropIndex("dbo.ChatMessage", new[] { "ReceiverId" });
            DropPrimaryKey("dbo.ChatMessage");
            AddColumn("dbo.ChatMessage", "Id", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.ChatMessage", "SenderId", c => c.String(maxLength: 128));
            AlterColumn("dbo.ChatMessage", "ReceiverId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.ChatMessage", "Id");
            CreateIndex("dbo.ChatMessage", "SenderId");
            CreateIndex("dbo.ChatMessage", "ReceiverId");
            AddForeignKey("dbo.ChatMessage", "SenderId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ChatMessage", "ReceiverId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatMessage", "ReceiverId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChatMessage", "SenderId", "dbo.AspNetUsers");
            DropIndex("dbo.ChatMessage", new[] { "ReceiverId" });
            DropIndex("dbo.ChatMessage", new[] { "SenderId" });
            DropPrimaryKey("dbo.ChatMessage");
            AlterColumn("dbo.ChatMessage", "ReceiverId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ChatMessage", "SenderId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.ChatMessage", "Id");
            AddPrimaryKey("dbo.ChatMessage", new[] { "SenderId", "ReceiverId" });
            CreateIndex("dbo.ChatMessage", "ReceiverId");
            CreateIndex("dbo.ChatMessage", "SenderId");
            AddForeignKey("dbo.ChatMessage", "ReceiverId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ChatMessage", "SenderId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
    }
}
