using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Security.Cryptography;
using LibraryService.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace LibraryService.Models
{
    public class BookRepository
    {
        private ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookDTO> GetBook(int bookId)
        {
            var book = await _context.Books.Select(b => new BookDTO
            {
                Author = b.Author,
                Available = b.PhysicalBooks.Any(pb => pb.UserId == null),
                BookId = b.Id,
                Title = b.Title
            }).FirstOrDefaultAsync();



            return book;
        }

        public async Task<List<BookDTO>> GetAllBooks()
        {
            var allBooks = await _context.Books.Select(b => new BookDTO
            {
                Author = b.Author,
                Available = b.PhysicalBooks.Any(pb => pb.UserId == null),
                BookId = b.Id,
                Title = b.Title
            }).ToListAsync();

            return allBooks;
        }

        public Task<List<CheckedOutBookDTO>> GetCheckedOutBooks(string userId)
        {
            var booksCheckedOut = from b in _context.Books
                                  join pb in _context.PhysicalBooks
                                      on b.Id equals pb.BookId
                                  where pb.UserId.Equals(userId)
                                  select new CheckedOutBookDTO
                                  {
                                      Author = b.Author,
                                      BookId = b.Id,
                                      PhysicalBookId = pb.Id,
                                      Title = b.Title,
                                      State = CheckedOutBookState.Valid
                                  };

            return booksCheckedOut.ToListAsync();
        }

        public async Task<CheckedOutBookDTO> CheckoutBook(int bookId, string userId)
        {
            var bookToCheckOut = await _context.Books
                .Include(b => b.PhysicalBooks)
                .Where(pb => pb.Id == bookId)
                .Select(b => new
            {
                BookId = b.Id,
                Author = b.Author,
                Title = b.Title,
                PhysicalBook = b.PhysicalBooks.FirstOrDefault(pb => pb.UserId == null)
            })
                .FirstOrDefaultAsync();

            if (bookToCheckOut == null)
            {
                return new CheckedOutBookDTO
                {
                    State = CheckedOutBookState.BookNotFound
                };
            }

            var checkedOutBookDTO = new CheckedOutBookDTO
            {
                Author = bookToCheckOut.Author,
                BookId = bookToCheckOut.BookId,
                Title = bookToCheckOut.Title
            };


            var physicalBook = bookToCheckOut.PhysicalBook;
            if (physicalBook == null)
            {
                checkedOutBookDTO.State = CheckedOutBookState.BookIsNotAvailable;
                return checkedOutBookDTO;
            }

            physicalBook.UserId = userId;
            await _context.SaveChangesAsync();
            checkedOutBookDTO.State = CheckedOutBookState.Valid;
            checkedOutBookDTO.PhysicalBookId = physicalBook.Id;

            return checkedOutBookDTO;
        }

        public async Task<CheckInBookDTO> CheckinBook(int bookId, string userId)
        {
            var checkInBookDTO = new CheckInBookDTO();

            var physicalBook = await _context.PhysicalBooks
                .FirstOrDefaultAsync(b => b.Book.Id == bookId && b.UserId == userId);
            if (physicalBook == null)
            {
                checkInBookDTO.State= CheckInBookDTO.CheckedInBookState.BookNotFound;
                return checkInBookDTO;
            }

            physicalBook.UserId = null;
            await _context.SaveChangesAsync();
            checkInBookDTO.State= CheckInBookDTO.CheckedInBookState.Valid;
            
            return checkInBookDTO;
        }
    }
}