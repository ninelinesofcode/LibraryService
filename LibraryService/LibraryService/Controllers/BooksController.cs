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

        // GET: api/Books/5
        [ResponseType(typeof(IEnumerable<CheckedOutBookViewModel>))]
        [Route("api/user/books")]
        public async Task<IHttpActionResult> GetCheckedOutBooks()
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            var booksCheckedOut= currentUser.BooksCheckedOut;
            
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