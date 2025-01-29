using BCMS_UI.Models;

namespace BCMS_UI.Services;

public class BookService : IBookService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
 
    public BookService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<BookDto> AddBookAsync(Book book)
    {
        try
        {
            var bookDto = new BookDto()
            {
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre
            };
            var response = await _httpClient.PostAsJsonAsync(_configuration["ApiUrl"] + "/api/books", bookDto);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BookDto>() ?? new BookDto();

        }
        catch (HttpRequestException e)
        {
            throw new BookServiceException("Unable to add book using the API.", e);
        }
        catch (Exception e)
        {
            throw new BookServiceException("An unexpected error occurred while adding the book.", e);
        }
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var response = await _httpClient.DeleteAsync(_configuration["ApiUrl"] + $"/api/books/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<BookDto?> GetBookAsync(int id)
    {
        var response = await _httpClient.GetAsync(_configuration["ApiUrl"] + $"/api/books/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<BookDto>();
    }

    public async Task<(List<BookDto> books, int totalCount)> GetBooksAsync(BookQueryParameters bookQueryParameters)
    {
        try
        {
            var queryString = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(bookQueryParameters.Title))
                queryString.Add("title", bookQueryParameters.Title);
            if (!string.IsNullOrEmpty(bookQueryParameters.Author))
                queryString.Add("author", bookQueryParameters.Author);
            if (!string.IsNullOrEmpty(bookQueryParameters.Genre))
                queryString.Add("genre", bookQueryParameters.Genre);

            queryString.Add("pageNumber", bookQueryParameters.PageNumber.ToString());
            queryString.Add("pageSize", bookQueryParameters.PageSize.ToString());

            if (!string.IsNullOrEmpty(bookQueryParameters.SortBy))
                queryString.Add("sortBy", bookQueryParameters.SortBy);

            var queryParams = string.Join("&", queryString.Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));

            var apiUrl = $"{_configuration["ApiUrl"]}/api/books?{queryParams}";

            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookDto>>();

            return (apiResponse?.books, apiResponse.totalCount);
        }
        catch (HttpRequestException e)
        {
            throw new BookServiceException("Unable to fetch books from the API.", e);
        }
        catch (Exception e)
        {
            throw new BookServiceException("An unexpected error occurred while fetching the books.", e);
        }
    }

    public async Task<BookDto> UpdateBookAsync(int id, Book book)
    {
        try
        {
            var bookDto = new BookDto()
            {
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre
            };

            var response = await _httpClient.PutAsJsonAsync(_configuration["ApiUrl"] + $"/api/books/{id}", bookDto);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BookDto>() ?? new BookDto();
        }
        catch (HttpRequestException e)
        {
            throw new BookServiceException("Unable to update book using the API.", e);
        }
        catch (Exception e)
        {
            throw new BookServiceException("An unexpected error occurred while updating the book.", e);
        }
    }

    public async Task<string> UploadBooksAsync(MultipartFormDataContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/books/import")
        {
            Content = content
        };
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public class ApiResponse<T>
    {
        public List<T> books { get; set; } = new List<T>();
        public int totalCount { get; set; }

    }
    public class BookServiceException : Exception
    {
        public BookServiceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
