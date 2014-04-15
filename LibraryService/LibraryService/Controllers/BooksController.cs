using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using LibraryService.Models;
using LibraryService.ViewModels;

namespace LibraryService.Controllers
{
    public class BooksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Books
        public IQueryable<BookViewModel> GetBooks()
        {
            return db.Books.Select(b => new BookViewModel { Author = b.Author, Title = b.Title, Id = b.Id });
        }

        // GET: api/Books/5
        [ResponseType(typeof(Book))]
        public IHttpActionResult GetBook(int id)
        {
            Book book = db.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.Id == id) > 0;
        }
    }
}