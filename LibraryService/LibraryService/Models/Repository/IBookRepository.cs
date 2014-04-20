using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryService.Services.DTO;

namespace LibraryService.Models
{
    public interface IBookRepository
    {
        Task<CheckInBookDTO> CheckinBook(int bookId, string userId);
        Task CheckoutBook(PhysicalBook physicalBook, string userId);
        Task<List<BookDTO>> GetAllBooks();
        Task<BookDTO> GetBook(int bookId);
        Task<List<CheckedOutBookDTO>> GetCheckedOutBooks(string userId);
    }
}