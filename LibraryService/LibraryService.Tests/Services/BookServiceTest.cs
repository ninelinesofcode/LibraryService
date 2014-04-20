using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using LibraryService.Models;
using LibraryService.Services.DTO;
using LibraryService.Services.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LibraryService.Tests.Services
{
    [TestClass]
    public class BookServiceTest
    {
        [TestMethod]
        public async Task GetAllBooksServiceCallsGetAllBooksRepository()
        {
            var bookServiceMock = new Mock<IBookRepository>();
            bookServiceMock.Setup(b => b.GetAllBooks())
                .ReturnsAsync(new List<BookDTO>());

            var bookService = new BooksService(bookServiceMock.Object);
            var result = await bookService.GetAllBooks();

            bookServiceMock.Verify(v => v.GetAllBooks(), Times.Once);
        }

        [TestMethod]
        public async Task GetCheckedOutBooksServiceCallsGetCheckoutBooksRepository()
        {
            var bookServiceMock = new Mock<IBookRepository>();
            bookServiceMock.Setup(b => b.GetCheckedOutBooks(It.IsAny<string>()))
                .ReturnsAsync(new List<CheckedOutBookDTO>());

            var principalMock = new Mock<IPrincipal>();
            principalMock.Setup(p => p.Identity.Name).Returns("test");

            var bookService = new BooksService(bookServiceMock.Object);
            var result = await bookService.GetCheckedOutBooks(principalMock.Object);

            bookServiceMock.Verify(v => v.GetCheckedOutBooks("test"), Times.Once);
        }
    }
}
