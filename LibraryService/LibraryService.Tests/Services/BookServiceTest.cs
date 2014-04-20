using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using LibraryService.Models;
using LibraryService.Services.DTO;
using LibraryService.Services.Implementation;
using LibraryService.ViewModels;
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
            var userServiceMock = new Mock<IUserService>();

            var bookService = new BooksService(bookServiceMock.Object, userServiceMock.Object);
            var result = await bookService.GetAllBooks();

            bookServiceMock.Verify(v => v.GetAllBooks(), Times.Once);
        }

        [TestMethod]
        public async Task GetCheckedOutBooksServiceCallsGetCheckoutBooksRepository()
        {
            var bookServiceMock = new Mock<IBookRepository>();
            bookServiceMock.Setup(b => b.GetCheckedOutBooks(It.IsAny<string>()))
                .ReturnsAsync(new List<CheckedOutBookDTO>());

            var userServiceMock = new Mock<IUserService>();

            var bookService = new BooksService(bookServiceMock.Object, userServiceMock.Object);

            userServiceMock.Setup(s => s.UserId).Returns("test");
            var result = await bookService.GetCheckedOutBooks();

            bookServiceMock.Verify(v => v.GetCheckedOutBooks("test"), Times.Once);
        }

        [TestMethod]
        public async Task GetCheckedOutBooksServiceReturnsViewModel()
        {
            var bookRepositoryMock = new Mock<IBookRepository>();
            bookRepositoryMock.Setup(b => b.GetCheckedOutBooks(It.IsAny<string>()))
                .ReturnsAsync(new List<CheckedOutBookDTO>
                {
                    new CheckedOutBookDTO()
                    {
                        Author = "test",
                        BookId = 1,
                        PhysicalBookId = 2,
                        State = CheckedOutBookState.Success,
                        Title = "test"
                    }
                });
            var expected = new CheckedOutBookViewModel()
            {
                Author = "test",
                BookId = 1,
                Title = "test",
                UserName = "test"
            };

            var userServiceMock = new Mock<IUserService>();

            userServiceMock.Setup(s => s.UserId).Returns("test");
            userServiceMock.Setup(s => s.UserName).Returns("test");

            var bookService = new BooksService(bookRepositoryMock.Object, userServiceMock.Object);

            var result = await bookService.GetCheckedOutBooks();
            var resultItem = result.First();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(expected.Author, resultItem.Author);
            Assert.AreEqual(expected.BookId, resultItem.BookId);
            Assert.AreEqual(expected.Title, resultItem.Title);
            Assert.AreEqual(expected.UserName, resultItem.UserName);
        }

        [TestMethod]
        public async Task CheckoutBookReturnsBookNotFoundWhenBookIsNotInRepository()
        {
            var bookRepositoryMock = new Mock<IBookRepository>();
            bookRepositoryMock.Setup(b => b.GetBook(It.IsAny<int>()))
                .ReturnsAsync(null);

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.UserId).Returns("test");
            userServiceMock.Setup(s => s.UserName).Returns("test");

            var bookService = new BooksService(bookRepositoryMock.Object, userServiceMock.Object);

            var result = await bookService.CheckOutBook(1);

            Assert.AreEqual(CheckedOutBookState.BookNotFound, result.State);
        }

        [TestMethod]
        public async Task CheckoutBookReturnsBookNotValidWhenThreeBooksAreCheckedOut()
        {
            var bookDTO = new BookDTO()
            {
                Author = "test",
                Available = true,
                BookId = 1,
                Title = "test"
            };
            bookDTO.PhysicalBooks = new List<PhysicalBook>() {
                new PhysicalBook() { BookId = bookDTO.BookId, Id=1}
                };
            var bookRepositoryMock = new Mock<IBookRepository>();
            bookRepositoryMock.Setup(b => b.GetBook(It.IsAny<int>()))
                .ReturnsAsync(bookDTO);
            bookRepositoryMock.Setup(b => b.GetCheckedOutBooks(It.IsAny<string>()))
                .ReturnsAsync(new List<CheckedOutBookDTO>()
                {
                    new CheckedOutBookDTO(),
                    new CheckedOutBookDTO(),
                    new CheckedOutBookDTO()
                });

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.UserId).Returns("test");
            userServiceMock.Setup(s => s.UserName).Returns("test");

            var bookService = new BooksService(bookRepositoryMock.Object, userServiceMock.Object);

            var result = await bookService.CheckOutBook(1);

            Assert.AreEqual(CheckedOutBookState.TooManyBooksCheckedOut, result.State);
        }

        [TestMethod]
        public async Task CheckoutBookReturnsBookNotValidWhenNoPhysicalBookAvailable()
        {
            var bookDTO = new BookDTO()
            {
                Author = "test",
                Available = true,
                BookId = 1,
                Title = "test"
            };
            bookDTO.PhysicalBooks = new List<PhysicalBook>() {
                new PhysicalBook() { BookId = bookDTO.BookId, Id=1, UserId = "test"}
                };
            var bookRepositoryMock = new Mock<IBookRepository>();
            bookRepositoryMock.Setup(b => b.GetBook(It.IsAny<int>()))
                .ReturnsAsync(bookDTO);
            bookRepositoryMock.Setup(b => b.GetCheckedOutBooks(It.IsAny<string>()))
                .ReturnsAsync(new List<CheckedOutBookDTO>()
                {
                    new CheckedOutBookDTO(),
                    new CheckedOutBookDTO()
                });

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.UserId).Returns("test");
            userServiceMock.Setup(s => s.UserName).Returns("test");

            var bookService = new BooksService(bookRepositoryMock.Object, userServiceMock.Object);

            var result = await bookService.CheckOutBook(1);

            Assert.AreEqual(CheckedOutBookState.BookIsNotAvailable, result.State);
        }

        [TestMethod]
        public async Task CheckoutBookReturnsSuccess()
        {
            var bookDTO = new BookDTO()
            {
                Author = "test",
                Available = true,
                BookId = 1,
                Title = "test"
            };
            bookDTO.PhysicalBooks = new List<PhysicalBook>() {
                new PhysicalBook() { BookId = bookDTO.BookId, Id=1, UserId = null}
                };
            var bookRepositoryMock = new Mock<IBookRepository>();
            bookRepositoryMock.Setup(b => b.GetBook(It.IsAny<int>()))
                .ReturnsAsync(bookDTO);
            bookRepositoryMock.Setup(b => b.GetCheckedOutBooks(It.IsAny<string>()))
                .ReturnsAsync(new List<CheckedOutBookDTO>()
                {
                    new CheckedOutBookDTO(),
                    new CheckedOutBookDTO()
                });

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.UserId).Returns("test");
            userServiceMock.Setup(s => s.UserName).Returns("test");

            var bookService = new BooksService(bookRepositoryMock.Object, userServiceMock.Object);

            var result = await bookService.CheckOutBook(1);

            Assert.AreEqual(CheckedOutBookState.Success, result.State);
        }
    }
}
