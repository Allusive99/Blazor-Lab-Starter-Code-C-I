using BlazorLibraryApp.Models;

namespace BlazorLibraryApp.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly string booksPath = "Data/Books.csv";
        private readonly string usersPath = "Data/Users.csv";
        private readonly string borrowedPath = "Data/BorrowedBooks.csv";

        private List<Book> books = new();
        private List<User> users = new();

        public List<Book> GetBooks() => books;
        public List<User> GetUsers() => users;

        public void ReadBooks()
        {
            Console.WriteLine("Reading from: " + Path.GetFullPath(booksPath));

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


        public void ReadUsers()
        {
            users.Clear();
            if (File.Exists(usersPath))
            {
                foreach (var line in File.ReadAllLines(usersPath))
                {
                    var parts = line.Split(',');
                    if (parts.Length == 3)
                    {
                        users.Add(new User
                        {
                            Id = int.Parse(parts[0]),
                            Name = parts[1],
                            Email = parts[2]
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

        public void AddUser(User user)
        {
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            WriteUsers();
        }

        public void EditUser(User user)
        {
            var existing = users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                existing.Name = user.Name;
                existing.Email = user.Email;
                WriteUsers();
            }
        }

        public void DeleteUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                users.Remove(user);
                WriteUsers();
            }
        }

        private void WriteBooks()
        {
            var lines = books.Select(b => $"{b.Id},{b.Title},{b.Author},{b.ISBN}");
            File.WriteAllLines(booksPath, lines);
        }

        private void WriteUsers()
        {
            var lines = users.Select(u => $"{u.Id},{u.Name},{u.Email}");
            File.WriteAllLines(usersPath, lines);
        }

        public Dictionary<int, List<Book>> ReadBorrowedBooks()
        {
            var result = new Dictionary<int, List<Book>>();

            if (File.Exists(borrowedPath))
            {
                foreach (var line in File.ReadAllLines(borrowedPath))
                {
                    var parts = line.Split(',');
                    if (parts.Length == 5)
                    {
                        int userId = int.Parse(parts[0]);
                        var book = new Book
                        {
                            Id = int.Parse(parts[1]),
                            Title = parts[2],
                            Author = parts[3],
                            ISBN = parts[4]
                        };

                        if (!result.ContainsKey(userId))
                            result[userId] = new List<Book>();

                        result[userId].Add(book);
                    }
                }
            }

            return result;
        }

        public void WriteBorrowedBooks(Dictionary<int, List<Book>> data)
        {
            Directory.CreateDirectory("Data"); // Ensure folder exists

            var lines = new List<string>();

            foreach (var entry in data)
            {
                foreach (var book in entry.Value)
                {
                    lines.Add($"{entry.Key},{book.Id},{book.Title},{book.Author},{book.ISBN}");
                }
            }

            File.WriteAllLines(borrowedPath, lines);
        }

        //  New method: remove book from Books.csv
        public void RemoveBookFromFile(int bookId)
        {
            if (File.Exists(booksPath))
            {
                var bookLines = File.ReadAllLines(booksPath).ToList();
                var updatedLines = bookLines
                    .Where(line =>
                    {
                        var parts = line.Split(',');
                        return parts.Length == 4 && int.Parse(parts[0]) != bookId;
                    });

                File.WriteAllLines(booksPath, updatedLines);
            }
        }

        //  New method: add book back to Books.csv
        public void AddBookToFile(Book book)
        {
            var line = $"{book.Id},{book.Title},{book.Author},{book.ISBN}";
            File.AppendAllLines(booksPath, new[] { line });
        }
    }
}
