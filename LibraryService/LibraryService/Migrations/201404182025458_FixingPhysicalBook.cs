namespace LibraryService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixingPhysicalBook : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PhysicalBooks", "Book_Id", "dbo.Books");
            DropIndex("dbo.PhysicalBooks", new[] { "Book_Id" });
            RenameColumn(table: "dbo.PhysicalBooks", name: "Book_Id", newName: "BookId");
            RenameColumn(table: "dbo.PhysicalBooks", name: "User_Id", newName: "UserId");
            RenameIndex(table: "dbo.PhysicalBooks", name: "IX_User_Id", newName: "IX_UserId");
            AlterColumn("dbo.PhysicalBooks", "BookId", c => c.Int(nullable: false));
            CreateIndex("dbo.PhysicalBooks", "BookId");
            AddForeignKey("dbo.PhysicalBooks", "BookId", "dbo.Books", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PhysicalBooks", "BookId", "dbo.Books");
            DropIndex("dbo.PhysicalBooks", new[] { "BookId" });
            AlterColumn("dbo.PhysicalBooks", "BookId", c => c.Int());
            RenameIndex(table: "dbo.PhysicalBooks", name: "IX_UserId", newName: "IX_User_Id");
            RenameColumn(table: "dbo.PhysicalBooks", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.PhysicalBooks", name: "BookId", newName: "Book_Id");
            CreateIndex("dbo.PhysicalBooks", "Book_Id");
            AddForeignKey("dbo.PhysicalBooks", "Book_Id", "dbo.Books", "Id");
        }
    }
}
