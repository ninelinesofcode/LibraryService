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
            Success,
            BookNotFound
        }

        public CheckedInBookState State { get; set; }
    }
}