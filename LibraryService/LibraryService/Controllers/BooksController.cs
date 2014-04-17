using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using LibraryService.Models;
using LibraryService.Services.Implementation;
using LibraryService.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LibraryService.Controllers
{
    [Authorize]
    public class BooksController : ApiController
    {
        private IBooksService booksService;

        public BooksController(IBooksService booksService)
        {
            this.booksService = booksService;
        }
        
        // GET: api/Books
        [AllowAnonymous]
        public IEnumerable<BookViewModel> GetBooks()
        {
            return booksService.GetAllBooks();
        }


        [ResponseType(typeof(IEnumerable<CheckedOutBookViewModel>))]
        [Route("api/user/books")]
        public async Task<IHttpActionResult> GetCheckedOutBooks()
        {
            var checkedOutBookViewModel = await booksService.GetCheckedOutBooks(User);

            return Ok(checkedOutBookViewModel);
        }

        [ResponseType(typeof(CheckedOutBookViewModel))]
        [Route("api/books/{bookId}/checkout/user")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckoutBook(int? bookId)
        {
            if (!bookId.HasValue)
            {
                return BadRequest("Invalid bookId");
            }

            var checkedOutBook = await booksService.CheckOutBook(bookId.Value, User);

            return Ok(checkedOutBook);
        }

        [Route("api/books/{bookId}/checkin/user")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckinBook(int? bookId)
        {
            if (!bookId.HasValue)
            {
                return BadRequest("Invalid bookId");
            }

            await booksService.CheckInBook(bookId.Value, User);
            return Ok();
        }
    }
}