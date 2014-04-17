using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using LibraryService.ViewModels;

namespace LibraryService.Services.Implementation
{
    public interface IBooksService
    {
        IEnumerable<BookViewModel> GetAllBooks();
        Task<IEnumerable<CheckedOutBookViewModel>> GetCheckedOutBooks(IPrincipal user);
        Task<CheckedOutBookViewModel> CheckOutBook(int bookId, IPrincipal user);
        Task CheckInBook(int bookId, IPrincipal user);
    }
}