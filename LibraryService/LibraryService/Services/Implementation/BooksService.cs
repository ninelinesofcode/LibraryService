using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using LibraryService.Services.DTO;
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

        public async Task<IEnumerable<BookDTO>> GetAllBooks()
        {
            var allBooks = await _repository.GetAllBooks();

            return allBooks;
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
                BookId = b.BookId.Value,
                Title = b.Title,
                UserName = user.Identity.GetUserName()
            }).ToList();

            return checkedOutBookViewModel;
        }

        public async Task<CheckedOutBookDTO> CheckOutBook(int bookId, IPrincipal user)
        {
            var userId = user.Identity.GetUserId();

            var booksCheckedOut = await _repository.GetCheckedOutBooks(userId);

            if (booksCheckedOut != null && booksCheckedOut.Count() >= 3)
            {
                return new CheckedOutBookDTO
                {
                    State = CheckedOutBookState.TooManyBooksCheckedOut
                };
            }

            var checkedOutBookDTO = await _repository.CheckoutBook(bookId, userId);

            return checkedOutBookDTO;
        }

        public async Task<CheckInBookDTO> CheckInBook(int bookId, IPrincipal user)
        {
            var userId = user.Identity.GetUserId();
            var checkInBookDTO = await _repository.CheckinBook(bookId, userId);
            return checkInBookDTO;

        }
    }
}