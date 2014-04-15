namespace LibraryService.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CheckoutBooks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PhysicalBooks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Book_Id = c.Int(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.Book_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.Book_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Author = c.String(nullable: false),
                        Title = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PhysicalBooks", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PhysicalBooks", "Book_Id", "dbo.Books");
            DropIndex("dbo.PhysicalBooks", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.PhysicalBooks", new[] { "Book_Id" });
            DropTable("dbo.Books");
            DropTable("dbo.PhysicalBooks");
        }
    }
}
