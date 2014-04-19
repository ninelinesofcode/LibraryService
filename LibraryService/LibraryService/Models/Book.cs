using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace LibraryService.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Title { get; set; }

        public  virtual ICollection<PhysicalBook> PhysicalBooks { get; set; }

    }
}