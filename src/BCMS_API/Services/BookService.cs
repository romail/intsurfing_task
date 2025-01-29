using BCMS_API.Models;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace BCMS_API.Services;

public class BookService : IBookService
{
    private readonly BookContext _context;
    private readonly IHubContext<BookHub> _hubContext;

    public BookService(BookContext context,IHubContext<BookHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    public async Task<BookDto> AddBookAsync(BookDto bookDto)
    {
        var book = new Book()
        {
            Title = bookDto.Title,
            Author = bookDto.Author,
            Genre = bookDto.Genre
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return new BookDto() { Id = book.Id, Title = book.Title, Author = book.Author, Genre = book.Genre };
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
        if (book == null)
        {
            return false;
        }
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
        return true;
    }

    public async Task<BookDto?> GetBookAsync(int id)
    {
        var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

        if (book == null)
            return null;

        return new BookDto() { Id = book.Id, Author = book.Author, Genre = book.Genre, Title = book.Title };
    }

    public async Task<BookResult> GetBooksAsync(BookQueryParameters bookQueryParameters)
    {
        var query = _context.Books.AsQueryable();

        query = ApplySearchFilters(query, bookQueryParameters);
        query = ApplySorting(query, bookQueryParameters);

        var totalCount = await query.CountAsync();
        var books = await query
                .Skip((bookQueryParameters.PageNumber - 1) * bookQueryParameters.PageSize)
                .Take(bookQueryParameters.PageSize)
                .Select(x => new BookDto() { Id = x.Id, Title = x.Title, Author = x.Author, Genre = x.Genre })
                .ToListAsync();

        return new BookResult { Books = books, TotalCount = totalCount };
    }

    private IQueryable<Book> ApplySearchFilters(IQueryable<Book> query, BookQueryParameters bookQueryParameters)
    {
        if (string.IsNullOrEmpty(bookQueryParameters.Title) && string.IsNullOrEmpty(bookQueryParameters.Author) && string.IsNullOrEmpty(bookQueryParameters.Genre))
        {
            return query;
        }

        var searchAll = !string.IsNullOrEmpty(bookQueryParameters.Title) && !string.IsNullOrEmpty(bookQueryParameters.Author) && !string.IsNullOrEmpty(bookQueryParameters.Genre);

        if (searchAll)
        {
            var searchTerm = bookQueryParameters.Title;
            query = query.Where(b =>
                        (b.Title != null && b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (b.Author != null && b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (b.Genre != null && b.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    );
        }
        else
        {
            if (!string.IsNullOrEmpty(bookQueryParameters.Title))
            {
                query = query.Where(b => b.Title != null && b.Title.Contains(bookQueryParameters.Title, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(bookQueryParameters.Author))
            {
                query = query.Where(b => b.Author != null && b.Author.Contains(bookQueryParameters.Author, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(bookQueryParameters.Genre))
            {
                query = query.Where(b => b.Genre != null && b.Genre.Contains(bookQueryParameters.Genre, StringComparison.OrdinalIgnoreCase));
            }
        }
        return query;
    }

    private IQueryable<Book> ApplySorting(IQueryable<Book> query, BookQueryParameters bookQueryParameters)
    {
        if (string.IsNullOrEmpty(bookQueryParameters.SortBy))
        {
            return query;
        }

        try
        {
            switch (bookQueryParameters.SortBy.ToLower())
            {
                case "title":
                    query = query.OrderBy(b => b.Title.ToLower());
                    break;
                case "author":
                    query = query.OrderBy(b => b.Author.ToLower());
                    break;
                case "genre":
                    query = query.OrderBy(b => b.Genre.ToLower());
                    break;
                default:
                    throw new ArgumentException($"Invalid sort field : {bookQueryParameters.SortBy}");
            }
        }
        catch (ArgumentException e)
        {
            Console.Error.WriteLine($"Error sorting books: {e.Message}");
        }

        return query;
    }

    public async Task<BookDto?> UpdateBookAsync(int id, BookDto bookDto)
    {
        var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

        if (book == null)
            return null;

        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.Genre = bookDto.Genre;

        await _context.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveUpdate");
        return new BookDto() { Id = book.Id, Title = book.Title, Author = book.Author, Genre = book.Genre };
    }

    private async Task<MemoryStream> GetBytes(IFormFile file)
    {
        var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    public async Task<bool> ImportBooksFromCsvAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return false;
        }

        try
        {
            var memoryStream = await GetBytes(file);
            using var reader = new StreamReader(memoryStream, Encoding.UTF8, true);
            reader.Peek();
            var encoding = reader.CurrentEncoding;

            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ","
            });

            csv.Read();
            csv.ReadHeader();

            var headers = csv.HeaderRecord;
            if (headers == null || headers.Length != 3 || headers[0] != "Title" || headers[1] != "Author" || headers[2] != "Genre")
            {
                return false;
            }

            var records = csv.GetRecords<BookImport>().ToList();

            if (records is null || !records.Any())
            {
                return false;
            }

            foreach (var rec in records)
            {
                if (rec is null || string.IsNullOrEmpty(rec.Title) || string.IsNullOrEmpty(rec.Author) || string.IsNullOrEmpty(rec.Genre))
                {
                    continue;
                }

                var book = new BookDto
                {
                    Title = rec.Title.Trim(),
                    Author = rec.Author.Trim(),
                    Genre = rec.Genre.Trim()
                };

                await AddBookAsync(book);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveUpdate");

            return true;
        }
        catch (CsvHelperException ex)
        {
            throw new Exception("An error occurred while reading the CSV file.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred while importing the books.", ex);
        }
    }
}


