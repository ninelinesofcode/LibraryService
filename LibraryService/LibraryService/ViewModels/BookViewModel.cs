using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryService.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
    }
}