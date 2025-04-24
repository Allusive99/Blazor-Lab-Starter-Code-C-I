using BlazorLibraryApp.Models;
using BlazorLibraryApp.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BlazorLibraryApp.Services
{
    public class LibraryServiceTestable : ILibraryService
    {
        private readonly string booksPath;
        private readonly string usersPath;

        private List<Book> books = new();
        private List<User> users = new();

        public LibraryServiceTestable(string booksPath, string usersPath)
        {
            this.booksPath = booksPath;
            this.usersPath = usersPath;
        }

        public List<Book> GetBooks() => books;
        public List<User> GetUsers() => users;

        public void ReadBooks()
        {
            books.Clear();
            if (File.Exists(booksPath))
            {
                foreach (var line in File.ReadAllLines(booksPath))
                {
                    var parts = line.Split(',');
                    if (parts.Length == 4)
                    {
                        books.Add(new Book
                        {
                            Id = int.Parse(parts[0]),
                            Title = parts[1],
                            Author = parts[2],
                            ISBN = parts[3]
                        });
                    }
                }
            }
        }

        public void AddBook(Book book)
        {
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            books.Add(book);
            WriteBooks();
        }

        public void EditBook(Book book)
        {
            var existing = books.FirstOrDefault(b => b.Id == book.Id);
            if (existing != null)
            {
                existing.Title = book.Title;
                existing.Author = book.Author;
                existing.ISBN = book.ISBN;
                WriteBooks();
            }
        }

        public void DeleteBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                books.Remove(book);
                WriteBooks();
            }
        }

        private void WriteBooks()
        {
            using (var writer = new StreamWriter(booksPath, false))
            {
                foreach (var b in books)
                {
                    writer.WriteLine($"{b.Id},{b.Title},{b.Author},{b.ISBN}");
                }
            }
        }



        // Stubbed methods (not under test)
        public void ReadUsers() { }
        public void AddUser(User user) { }
        public void EditUser(User user) { }
        public void DeleteUser(int id) { }

        public Dictionary<int, List<Book>> ReadBorrowedBooks() => new();
        public void WriteBorrowedBooks(Dictionary<int, List<Book>> data) { }
        public void RemoveBookFromFile(int bookId) { }
        public void AddBookToFile(Book book) { }
    }
}
