using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using LibraryService.Services.DTO;
using LibraryService.ViewModels;

namespace LibraryService.Services.Implementation
{
    public interface IBooksService
    {
        Task<IEnumerable<BookDTO>> GetAllBooks();
        Task<IEnumerable<CheckedOutBookViewModel>> GetCheckedOutBooks();
        Task<CheckedOutBookDTO> CheckOutBook(int bookId);
        Task<CheckInBookDTO> CheckInBook(int bookId);
    }
}