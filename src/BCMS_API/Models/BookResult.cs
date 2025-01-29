using BCMS_API.Models;

public class BookResult
{
    public IEnumerable<BookDto> Books { get; set; }
    public int TotalCount { get; set; }
}