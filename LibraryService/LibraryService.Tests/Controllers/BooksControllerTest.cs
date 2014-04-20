using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Http.Results;
using System.Web.Mvc;
using LibraryService.Controllers;
using LibraryService.Services.DTO;
using LibraryService.Services.Implementation;
using LibraryService.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;

namespace LibraryService.Tests.Controllers
{
    [TestClass]
    public class BooksControllerTest
    {
        [TestMethod]
        public async Task CheckOutBooksRetunsBadRequestWithABadBookId()
        {
            var bookService = new Mock<IBooksService>();
            var controller = new BooksController(bookService.Object);

            var result = await controller.CheckoutBook(null) as BadRequestErrorMessageResult;

            Assert.AreEqual("Invalid bookId", result.Message);
        }

        [TestMethod]
        public async Task CheckedOutBooksReturnsTooManyBooksCheckedOut()
        {
            var bookServiceMock = new Mock<IBooksService>();
            bookServiceMock.Setup(bs => bs.CheckOutBook(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CheckedOutBookDTO { State = CheckedOutBookState.TooManyBooksCheckedOut });


            var controller = new BooksController(bookServiceMock.Object);

            var result = await controller.CheckoutBook(1) as BadRequestErrorMessageResult;

            Assert.AreEqual("User has too many books checked out", result.Message);
        }

        [TestMethod]
        public async Task CheckedOutBooksReturnsBookIsNotAvailable()
        {
            var bookServiceMock = new Mock<IBooksService>();
            bookServiceMock.Setup(bs => bs.CheckOutBook(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CheckedOutBookDTO { State = CheckedOutBookState.BookIsNotAvailable });


            var controller = new BooksController(bookServiceMock.Object);

            var result = await controller.CheckoutBook(1) as BadRequestErrorMessageResult;

            Assert.AreEqual("This book has no more copies to check out", result.Message);
        }

        [TestMethod]
        public async Task CheckedOutBooksReturnsBookNotFound()
        {
            var bookServiceMock = new Mock<IBooksService>();
            bookServiceMock.Setup(bs => bs.CheckOutBook(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CheckedOutBookDTO { State = CheckedOutBookState.BookNotFound });


            var controller = new BooksController(bookServiceMock.Object);

            var result = await controller.CheckoutBook(1) as BadRequestErrorMessageResult;

            Assert.AreEqual("This book does not exist at this library", result.Message);
        }

        [TestMethod]
        public async Task CheckedOutBooksReturnsSuccess()
        {
            var bookServiceMock = new Mock<IBooksService>();
            var checkedOutBook = new CheckedOutBookDTO
            {
                State = CheckedOutBookState.Success
            };
            bookServiceMock.Setup(bs => bs.CheckOutBook(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(checkedOutBook);


            var controller = new BooksController(bookServiceMock.Object);

            var result = await controller.CheckoutBook(1) as OkResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task CheckInBookReturnsBadRequestWithNullBookId()
        {
            var bookServiceMock = new Mock<IBooksService>();
            var controller = new BooksController(bookServiceMock.Object);

            var result = await controller.CheckinBook(null) as BadRequestErrorMessageResult;

            Assert.AreEqual("Invalid bookId", result.Message);
        }

        [TestMethod]
        public async Task CheckInBookReturnsBadRequestWhenBookNotFound()
        {
            var bookServiceMock = new Mock<IBooksService>();
            bookServiceMock.Setup(bs => bs.CheckInBook(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CheckInBookDTO
            {
                State = CheckInBookDTO.CheckedInBookState.BookNotFound
            });

            var controller = new BooksController(bookServiceMock.Object);

            var result = await controller.CheckinBook(1) as BadRequestErrorMessageResult;

            Assert.AreEqual("1 is not checked out to the user", result.Message);
        }

        [TestMethod]
        public async Task CheckInBookReturnsOkWhenSuccess()
        {
            var bookServiceMock = new Mock<IBooksService>();
            bookServiceMock.Setup(bs => bs.CheckInBook(It.IsAny<int>(), It.IsAny<IPrincipal>()))
                .ReturnsAsync(new CheckInBookDTO
            {
                State = CheckInBookDTO.CheckedInBookState.Success
            });

            var controller = new BooksController(bookServiceMock.Object);

            var result = await controller.CheckinBook(1) as OkResult;

            Assert.IsNotNull(result);
        }
    }
}
