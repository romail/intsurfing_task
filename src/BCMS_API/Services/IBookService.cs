using BCMS_API.Models;

namespace BCMS_API.Services;

public interface IBookService
{
    Task<BookResult> GetBooksAsync(BookQueryParameters bookQueryParameters);
    Task<BookDto?> GetBookAsync(int id);
    Task<BookDto> AddBookAsync(BookDto bookDto);
    Task<BookDto?> UpdateBookAsync(int id, BookDto bookDto);
    Task<bool> DeleteBookAsync(int id);
    Task<bool> ImportBooksFromCsvAsync(IFormFile file);
}
