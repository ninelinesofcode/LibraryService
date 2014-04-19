using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using LibraryService.ViewModels;
using LibraryService.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LibraryService.Services.Implementation
{
    public class BooksService : IBooksService
    {
        private readonly ApplicationDbContext _context;
        private BookRepository _repository;


        public BooksService(ApplicationDbContext context, BookRepository repository)
        {
            _repository = repository;
            _context = context;
        }

        public IEnumerable<BookViewModel> GetAllBooks()
        {

            return _context.Books.Select(b => new BookViewModel { Author = b.Author, Title = b.Title, Id = b.Id }).AsEnumerable();
        }

        public async Task<IEnumerable<CheckedOutBookViewModel>> GetCheckedOutBooks(IPrincipal user)
        {
            var currentUserId = user.Identity.GetUserId();
            var booksCheckedOut = await _repository.GetCheckedOutBooks(currentUserId);

            var checkedOutBookViewModel =
                booksCheckedOut.Select(
                    b =>
                        new CheckedOutBookViewModel
            {
                Author = b.Author,
                BookId = b.BookId,
                Title = b.Title,
                UserName = user.Identity.GetUserName()
            }).ToList();

            return checkedOutBookViewModel;
        }

        public async Task<CheckedOutBookViewModel> CheckOutBook(int bookId, IPrincipal user)
        {
            var userId = user.Identity.GetUserId();

            var booksCheckedOut = _context.PhysicalBooks
                .Where(b => b.Book.Id == bookId && b.UserId == userId);

            if (booksCheckedOut != null && booksCheckedOut.Count() >= 3)
            {
                //return BadRequest("The users already has 3 books checked out");
                return null;
            }

            var book = await _context.Books

                .FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                //return NotFound();
                return null;
            }

            var physicalBook = book.PhysicalBooks
                .FirstOrDefault(p => p.User == null);
            if (physicalBook == null)
            {
                //return BadRequest("Book is already checked out");
                return null;
            }

            physicalBook.UserId = userId;
            await _context.SaveChangesAsync();

            var checkedOutBook = new CheckedOutBookViewModel
            {
                Author = book.Author,
                BookId = book.Id,
                Title = book.Title,
                UserName = user.Identity.GetUserName()
            };

            return checkedOutBook;
        }

        public async Task CheckInBook(int bookId, IPrincipal user)
        {
            var userId = user.Identity.GetUserId();
            var physicalBook = await _context.PhysicalBooks
                .Include(p => p.User)
                .FirstOrDefaultAsync(b => b.Book.Id == bookId && b.User.Id == userId);
            if (physicalBook == null)
            {
                return;

                //return NotFound();
            }

            physicalBook.User = null;
            await _context.SaveChangesAsync();
        }
    }
}