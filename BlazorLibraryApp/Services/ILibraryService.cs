using BlazorLibraryApp.Models;

namespace BlazorLibraryApp.Services
{
    public interface ILibraryService
    {
        List<Book> GetBooks();
        List<User> GetUsers();

        void ReadBooks();
        void ReadUsers();

        void AddBook(Book book);
        void EditBook(Book book);
        void DeleteBook(int id);

        void AddUser(User user);
        void EditUser(User user);
        void DeleteUser(int id);

        Dictionary<int, List<Book>> ReadBorrowedBooks();
        void WriteBorrowedBooks(Dictionary<int, List<Book>> data);

        // New methods for book file updates
        void RemoveBookFromFile(int bookId);
        void AddBookToFile(Book book);
    }
}
