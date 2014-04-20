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
    public class BookRepository : IBookRepository
    {
        private ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookDTO> GetBook(int bookId)
        {
            var book = await _context.Books
                .Where(b => b.Id == bookId)
                .Select(b => new BookDTO
            {
                Author = b.Author,
                Available = b.PhysicalBooks.Any(pb => pb.UserId == null),
                BookId = b.Id,
                Title = b.Title,
                PhysicalBooks = b.PhysicalBooks
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
                                      State = CheckedOutBookState.Success
                                  };

            return booksCheckedOut.ToListAsync();
        }

        public async Task CheckoutBook(PhysicalBook physicalBook, string userId)
        {
            physicalBook.UserId = userId;
            await _context.SaveChangesAsync();
        }

        public async Task<CheckInBookDTO> CheckinBook(int bookId, string userId)
        {
            var checkInBookDTO = new CheckInBookDTO();

            var physicalBook = await _context.PhysicalBooks
                .FirstOrDefaultAsync(b => b.Book.Id == bookId && b.UserId == userId);
            if (physicalBook == null)
            {
                checkInBookDTO.State = CheckInBookDTO.CheckedInBookState.BookNotFound;
                return checkInBookDTO;
            }

            physicalBook.UserId = null;
            await _context.SaveChangesAsync();
            checkInBookDTO.State = CheckInBookDTO.CheckedInBookState.Success;

            return checkInBookDTO;
        }
    }
}