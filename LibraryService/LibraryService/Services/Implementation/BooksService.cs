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

        private readonly UserManager<ApplicationUser> _manager;

        public BooksService(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _manager = manager;
            _context = context;
        }
        
        public IEnumerable<BookViewModel> GetAllBooks()
        {

            return _context.Books.Select(b => new BookViewModel { Author = b.Author, Title = b.Title, Id = b.Id }).AsEnumerable();
        }

        public async Task<IEnumerable<CheckedOutBookViewModel>> GetCheckedOutBooks(IPrincipal user)
        {
            var currentUser = await _manager.FindByIdAsync(user.Identity.GetUserId());
            var booksCheckedOut = currentUser.BooksCheckedOut;

            if (booksCheckedOut == null)
            {
                return null;
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

            return checkedOutBookViewModel;
        }

        public async Task<CheckedOutBookViewModel> CheckOutBook(int bookId, IPrincipal user)
        {
            var userId = user.Identity.GetUserId();

            var booksCheckedOut = _context.PhysicalBooks
                .Include(p => p.User)
                .Where(b => b.Book.Id == bookId && b.User.Id == userId);

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

            physicalBook.User = await _manager.FindByIdAsync(userId);
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