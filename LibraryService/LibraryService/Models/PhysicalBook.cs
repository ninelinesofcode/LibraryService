using Microsoft.AspNet.Identity;

namespace LibraryService.Models
{
    public class PhysicalBook
    {
        public int Id { get; set; }
        public IUser CheckedOutToUser { get; set; }
        public Book Book { get; set; }
    }
}