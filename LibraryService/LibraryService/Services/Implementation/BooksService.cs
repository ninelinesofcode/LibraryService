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
        private IBookRepository _repository;
        private IUserService _userService;

        public BooksService(IBookRepository repository, IUserService userService)
        {
            _userService = userService;
            _repository = repository;
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooks()
        {
            var allBooks = await _repository.GetAllBooks();

            return allBooks;
        }

        public async Task<IEnumerable<CheckedOutBookViewModel>> GetCheckedOutBooks()
        {
            var currentUserId = _userService.UserId;
            var booksCheckedOut = await _repository.GetCheckedOutBooks(currentUserId);

            var checkedOutBookViewModel =
                booksCheckedOut.Select(
                    b =>
                        new CheckedOutBookViewModel
            {
                Author = b.Author,
                BookId = b.BookId.Value,
                Title = b.Title,
                UserName = _userService.UserName
            }).ToList();

            return checkedOutBookViewModel;
        }

        public async Task<CheckedOutBookDTO> CheckOutBook(int bookId)
        {
            var userId = _userService.UserId;

            var book = await _repository.GetBook(bookId);
            if (book == null)
            {
                return new CheckedOutBookDTO { State = CheckedOutBookState.BookNotFound };
            }

            var checkedOutBookDTO = new CheckedOutBookDTO
            {
                Author = book.Author,
                BookId = book.BookId,
                Title = book.Title
            };

            var booksCheckedOut = await _repository.GetCheckedOutBooks(userId);

            if (booksCheckedOut != null && booksCheckedOut.Count() >= 3)
            {
                checkedOutBookDTO.State = CheckedOutBookState.TooManyBooksCheckedOut;
                return checkedOutBookDTO;
            }

            var physicalBookToCheckOut = book.PhysicalBooks.FirstOrDefault(pb => pb.UserId == null);
            if (physicalBookToCheckOut == null)
            {
                checkedOutBookDTO.State = CheckedOutBookState.BookIsNotAvailable;
                return checkedOutBookDTO;
            }

            await _repository.CheckoutBook(physicalBookToCheckOut, userId);
            checkedOutBookDTO.State = CheckedOutBookState.Success;
            checkedOutBookDTO.PhysicalBookId = physicalBookToCheckOut.Id;

            return checkedOutBookDTO;
        }

        public async Task<CheckInBookDTO> CheckInBook(int bookId)
        {
            var userId = _userService.UserId;
            var checkInBookDTO = await _repository.CheckinBook(bookId, userId);
            return checkInBookDTO;

        }
    }
}