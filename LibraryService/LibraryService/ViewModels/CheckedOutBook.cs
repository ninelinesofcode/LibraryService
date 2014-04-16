using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryService.ViewModels
{
    public class CheckedOutBookViewModel
    {
        public int BookId { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
    }
}