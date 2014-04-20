using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryService.Services.DTO
{
    public class CheckInBookDTO
    {
        public enum CheckedInBookState
        {
            Valid,
            BookNotFound
        }

        public CheckedInBookState State { get; set; }
    }
}