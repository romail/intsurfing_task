namespace BCMS_API;

using BCMS_API.Models;
using Microsoft.EntityFrameworkCore;

public class BookContext(DbContextOptions<BookContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; } 

    public void SeedDatabase()
    {
        if (!Books.Any())
        {
            Books.AddRange(
                new Book
                {
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    Genre = "Fiction"
                },
                new Book
                {
                    Title = "1984",
                    Author = "George Orwell",
                    Genre = "Dystopian"
                },
                new Book
                {
                    Title = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    Genre = "Fiction"
                },
                new Book
                {
                    Title = "The Catcher in the Rye",
                    Author = "J.D. Salinger",
                    Genre = "Fiction"
                },
                new Book
                {
                    Title = "The Hobbit",
                    Author = "J.R.R. Tolkien",
                    Genre = "Fantasy"
                },
                new Book
                {
                    Title = "Pride and Prejudice",
                    Author = "Jane Austen",
                    Genre = "Romance"
                },
                new Book
                {
                    Title = "Moby-Dick",
                    Author = "Herman Melville",
                    Genre = "Adventure"
                },
                new Book
                {
                    Title = "War and Peace",
                    Author = "Leo Tolstoy",
                    Genre = "Historical Fiction"
                },
                new Book
                {
                    Title = "The Odyssey",
                    Author = "Homer",
                    Genre = "Epic"
                }
            );

            SaveChanges();
        }
    }
}
