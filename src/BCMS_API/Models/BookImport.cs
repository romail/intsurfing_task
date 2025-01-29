using CsvHelper.Configuration.Attributes;

public class BookImport
{
    [Name("Title")]
    public string? Title { get; set; }
    [Name("Author")]
    public string? Author { get; set; }
    [Name("Genre")]
    public string? Genre { get; set; }
}