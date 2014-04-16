using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using LibraryService.Models;
using LibraryService.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LibraryService.Controllers
{
    [Authorize]
    public class BooksController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> manager;

        public BooksController()
        {
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: api/Books
        [AllowAnonymous]
        public IQueryable<BookViewModel> GetBooks()
        {
            return db.Books.Select(b => new BookViewModel { Author = b.Author, Title = b.Title, Id = b.Id });
        }


        [ResponseType(typeof(IEnumerable<CheckedOutBookViewModel>))]
        [Route("api/user/books")]
        public async Task<IHttpActionResult> GetCheckedOutBooks()
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            var booksCheckedOut = currentUser.BooksCheckedOut;

            if (booksCheckedOut == null)
            {
                return NotFound();
            }

            var checkedOutBookViewModel =
                booksCheckedOut.Select(
                    b =>
                        new CheckedOutBookViewModel
            {
                Author = b.Book.Author,
                BookId = b.Book.Id,
                Title = b.Book.Title,
                UserName = currentUser.UserName
            });

            return Ok(checkedOutBookViewModel);
        }

        [ResponseType(typeof(IEnumerable<CheckedOutBookViewModel>))]
        [Route("api/books/{bookId}/checkout/user")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckoutBook(int? bookId)
        {
            if (!bookId.HasValue)
            {
                return BadRequest("Invalid bookId");
            }

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            var booksCheckedOut = currentUser.BooksCheckedOut;

            if (booksCheckedOut != null && booksCheckedOut.Count >= 3)
            {
                return BadRequest("The users already has 3 books checked out");
            }

            var book = await db.Books
                .FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                return NotFound();
            }

            var physicalBook = book.PhysicalBooks
                .FirstOrDefault(p => p.User == null);
            if (physicalBook == null)
            {
                return BadRequest("Book is already checked out");
            }

            physicalBook.User = currentUser;
            await db.SaveChangesAsync();

            return Ok();
        }

        [ResponseType(typeof(IEnumerable<CheckedOutBookViewModel>))]
        [Route("api/books/{bookId}/checkin/user")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckinBook(int? bookId)
        {
            if (!bookId.HasValue)
            {
                return BadRequest("Invalid bookId");
            }

            var userId = User.Identity.GetUserId();
            var physicalBook = await db.PhysicalBooks
                .Include(p => p.User)
                .FirstOrDefaultAsync(b => b.Book.Id == bookId && b.User.Id == userId);
            if (physicalBook == null)
            {
                return NotFound();
            }

            physicalBook.User = null;
            await db.SaveChangesAsync();

            return Ok();
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