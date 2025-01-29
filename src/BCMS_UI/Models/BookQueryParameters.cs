namespace BCMS_UI.Models;

public class BookQueryParameters
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Genre { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
}
