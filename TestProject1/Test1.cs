using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlazorLibraryApp.Models;
using BlazorLibraryApp.Services;
using System.IO;
using System.Linq;

namespace TestProject1
{
    [TestClass]
    public class Test1
    {
        private string booksPath = "";
        private string usersPath = "";
        private LibraryServiceTestable? service;

        [TestInitialize]
        public void Setup()
        {
            // Create temp file paths
            booksPath = Path.Combine(Path.GetTempPath(), $"TestBooks_{Guid.NewGuid()}.csv");
            usersPath = Path.Combine(Path.GetTempPath(), $"TestUsers_{Guid.NewGuid()}.csv");

            // Write test CSV
            File.WriteAllLines(booksPath, new[]
            {
                "1,Book A,Author A,111",
                "2,Book B,Author B,222"
            });

            service = new LibraryServiceTestable(booksPath, usersPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                if (File.Exists(booksPath)) File.Delete(booksPath);
                if (File.Exists(usersPath)) File.Delete(usersPath);
            }
            catch (IOException ex)
            {
                Assert.Fail("Cleanup failed: " + ex.Message);
            }
        }

        [TestMethod]
        public void ReadBooks_ShouldLoadBooksFromCSV()
        {
            // Act
            service!.ReadBooks();
            var books = service.GetBooks();

            // Assert
            Assert.AreEqual(3, books.Count);
            Assert.AreEqual("Book A", books[0].Title);
        }

        [TestMethod]
        public void AddBook_ShouldAddNewBookAndSave()
        {
            // Arrange
            service!.ReadBooks();
            var newBook = new Book { Title = "Book C", Author = "Author C", ISBN = "333" };

            // Act
            service.AddBook(newBook);

            // Assert
            Assert.AreEqual(3, service.GetBooks().Count);
            Assert.IsTrue(service.GetBooks().Any(b => b.Title == "Book C"));
        }

        [TestMethod]
        public void EditBook_ShouldUpdateBookFields()
        {
            // Arrange
            service!.ReadBooks();
            var book = service.GetBooks().First();
            book.Title = "Updated Title";

            // Act
            service.EditBook(book);

            // Assert
            Assert.AreEqual("Updated Title", service.GetBooks().First().Title);
        }

        [TestMethod]
        public void DeleteBook_ShouldRemoveBookFromListAndFile()
        {
            // Arrange
            service!.ReadBooks();
            int idToDelete = service.GetBooks().First().Id;

            // Act
            service.DeleteBook(idToDelete);

            // Assert
            Assert.AreEqual(1, service.GetBooks().Count);
            Assert.IsFalse(service.GetBooks().Any(b => b.Id == idToDelete));
        }
    }
}
// Triggering CI workflow test