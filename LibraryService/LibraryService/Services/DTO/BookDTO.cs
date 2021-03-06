﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibraryService.Models;

namespace LibraryService.Services.DTO
{
    public class BookDTO
    {
        public int BookId { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public bool Available { get; set; }
        public ICollection<PhysicalBook> PhysicalBooks { get; set; } 
    }
}