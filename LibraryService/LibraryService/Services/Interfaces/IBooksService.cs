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
        Task<IEnumerable<CheckedOutBookViewModel>> GetCheckedOutBooks(IPrincipal user);
        Task<CheckedOutBookDTO> CheckOutBook(int bookId, IPrincipal user);
        Task<CheckInBookDTO> CheckInBook(int bookId, IPrincipal user);
    }
}