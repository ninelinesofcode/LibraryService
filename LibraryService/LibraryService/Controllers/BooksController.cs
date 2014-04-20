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
using LibraryService.Services.DTO;
using LibraryService.Services.Implementation;
using LibraryService.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LibraryService.Controllers
{
    public class BooksController : ApiController
    {
        private IBooksService booksService;
        
        public BooksController(IBooksService booksService)
        {
            this.booksService = booksService;
        }

        // GET: api/Books
        [AllowAnonymous]
        public async Task<IEnumerable<BookViewModel>> GetBooks()
        {
            var allBooks = await booksService.GetAllBooks();
            var allBooksViewModel = allBooks.Select(b =>
                new BookViewModel
            {
                Author = b.Author,
                Title = b.Title,
                BookId = b.BookId,
                Available = b.Available
            }).ToList();
            return allBooksViewModel;
        }

        [Route("api/user/books")]
        public async Task<IEnumerable<CheckedOutBookViewModel>> GetCheckedOutBooks()
        {
            var checkedOutBookViewModel = await booksService.GetCheckedOutBooks();

            return checkedOutBookViewModel;
        }
        
        [Route("api/books/{bookId}/checkout/user")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckoutBook(int? bookId)
        {
            if (!bookId.HasValue)
            {
                return BadRequest("Invalid bookId");
            }

            var checkedOutBook = await booksService.CheckOutBook(bookId.Value);
            if (checkedOutBook.State == CheckedOutBookState.TooManyBooksCheckedOut)
            {
                return BadRequest("User has too many books checked out");
            }

            if (checkedOutBook.State == CheckedOutBookState.BookIsNotAvailable)
            {
                return BadRequest("This book has no more copies to check out");
            }

            if (checkedOutBook.State == CheckedOutBookState.BookNotFound)
            {
                return BadRequest("This book does not exist at this library");
            }

            if (checkedOutBook.State != CheckedOutBookState.Success)
            {
                return BadRequest("Unknown error");
            }

            //Should return Created but not sure where it should point to
            //return Created<CheckedOutBookViewModel>(string.Empty, null);
            return Ok();
        }

        [Route("api/books/{bookId}/checkin/user")]
        [HttpPost]
        public async Task<IHttpActionResult> CheckinBook(int? bookId)
        {
            if (!bookId.HasValue)
            {
                return BadRequest("Invalid bookId");
            }

            var checkInBookDTO = await booksService.CheckInBook(bookId.Value);
            if (checkInBookDTO.State == CheckInBookDTO.CheckedInBookState.BookNotFound)
            {
                return BadRequest(string.Format("{0} is not checked out to the user", bookId));
            }

            if (checkInBookDTO.State != CheckInBookDTO.CheckedInBookState.Success)
            {
                return BadRequest("Bad Request");
            }
            return Ok();
        }
    }
}