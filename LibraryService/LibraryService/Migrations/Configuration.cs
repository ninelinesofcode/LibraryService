using System.Collections.Generic;

namespace LibraryService.Migrations
{
    using LibraryService.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LibraryService.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(LibraryService.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            SeedUsers(context);
            SeedBooks(context);
        }

        private void SeedBooks(LibraryService.Models.ApplicationDbContext context)
        {
            context.Books.AddOrUpdate(b => b.Title,
                new Book { Author = "Piers Anthony", Title = "A Spell for Chameleon", PhysicalBooks = new List<PhysicalBook> { new PhysicalBook() } },
                new Book { Author = "Charles Dickens", Title = "A Tale of Two Cities", PhysicalBooks = new List<PhysicalBook> { new PhysicalBook() } },
                new Book { Author = "J. R. R. Tolkein", Title = "The Lord of the Rings", PhysicalBooks = new List<PhysicalBook> { new PhysicalBook() } },
                new Book { Author = "J. K. Rowling", Title = @"Harry Potter and the Philosopher's Stone", PhysicalBooks = new List<PhysicalBook> { new PhysicalBook() } },
                new Book { Author = "J. R. R. Tolkein", Title = "The Hobbit", PhysicalBooks = new List<PhysicalBook> { new PhysicalBook() } },
                new Book { Author = "Agatha Christie", Title = "And Then There Were None", PhysicalBooks = new List<PhysicalBook> { new PhysicalBook() } }

            );


        }
        private void SeedUsers(LibraryService.Models.ApplicationDbContext context)
        {
            var manager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(
                    context));

            for (int i = 0; i < 4; i++)
            {
                var user = new ApplicationUser()
                {
                    UserName = string.Format("User{0}", i.ToString())
                };
                manager.Create(user, string.Format("Password{0}", i.ToString()));
            }
        }


    }
}

