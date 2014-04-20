using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryService.Services.DTO
{
    public enum CheckedOutBookState
    {
        Success,
        BookNotFound,
        TooManyBooksCheckedOut,
        BookIsNotAvailable
    }
    public class CheckedOutBookDTO
    {
        public int? PhysicalBookId { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public CheckedOutBookState State { get; set; }
        public int? BookId { get; set; }
    }
}