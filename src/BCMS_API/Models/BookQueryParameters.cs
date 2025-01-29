namespace BCMS_API.Models;

public class BookQueryParameters : IParsable<BookQueryParameters>
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Genre { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }

    public static BookQueryParameters Parse(string s, IFormatProvider? provider)
    {
        var parameters = new BookQueryParameters();
        if (string.IsNullOrEmpty(s))
        {
            return parameters;
        }

        var queryParts = s.Split('&');

        foreach (var part in queryParts)
        {
            var keyValue = part.Split('=');

            if (keyValue.Length != 2)
                continue;
            var key = keyValue[0];
            var value = keyValue[1];

            switch (key.ToLower())
            {
                case "title":
                    parameters.Title = value;
                    break;
                case "author":
                    parameters.Author = value;
                    break;
                case "genre":
                    parameters.Genre = value;
                    break;
                case "pagenumber":
                    if (int.TryParse(value, out int pageNumber))
                        parameters.PageNumber = pageNumber;
                    break;
                case "pagesize":
                        if (int.TryParse(value, out int pageSize))
                            parameters.PageSize = pageSize;
                    break;
                case "sortby":
                    parameters.SortBy = value;
                    break;

            }
        }
        return parameters;
    }
    public static bool TryParse(string? s, IFormatProvider? provider, out BookQueryParameters result)
    {
        result = null!;
        if(string.IsNullOrEmpty(s)) return false;
        result = Parse(s,provider);

        return true;
    }
}
