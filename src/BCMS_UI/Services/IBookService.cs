using BCMS_UI.Models;

namespace BCMS_UI.Services;

public interface IBookService
{
    Task<(List<BookDto> books, int totalCount)> GetBooksAsync(BookQueryParameters bookQueryParameters);
    Task<BookDto?> GetBookAsync(int id);
    Task<BookDto> AddBookAsync(Book book);
    Task<BookDto> UpdateBookAsync(int id, Book book);
    Task<bool> DeleteBookAsync(int id);
    Task<string> UploadBooksAsync(MultipartFormDataContent content);
}