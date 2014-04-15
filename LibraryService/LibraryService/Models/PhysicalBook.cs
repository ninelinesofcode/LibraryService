using Microsoft.AspNet.Identity;

namespace LibraryService.Models
{
    public class PhysicalBook
    {
        public int Id { get; set; }
        public virtual IUser CheckedOutToUser { get; set; }
        public virtual Book Book { get; set; }
    }
}