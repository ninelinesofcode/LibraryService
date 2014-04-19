using System.Data.Entity;
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
    }
}