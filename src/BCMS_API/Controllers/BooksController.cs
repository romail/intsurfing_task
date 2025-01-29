using BCMS_API.Models;
using BCMS_API.Services;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace BCMS_API.Controllers;

public static class BooksController
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/books").WithTags("Books");

        group.MapGet("/", GetBooks).RequireRateLimiting("fixed");
        group.MapGet("/{id}", GetBook).RequireRateLimiting("fixed");
        group.MapPost("/", CreateBook).RequireRateLimiting("fixed");
        group.MapPut("/{id}", UpdateBook).RequireRateLimiting("fixed");
        group.MapDelete("/{id}", DeleteBook).RequireRateLimiting("fixed");
        group.MapPost("/import", ImportBooks).Accepts<IFormFile>("multipart/form-data").DisableAntiforgery().RequireRateLimiting("fixed");
    }

    private static async Task<IResult> GetBooks([AsParameters] BookQueryParameters bookQueryParameters, IBookService bookService)
    {
        var books = await bookService.GetBooksAsync(bookQueryParameters);
        return Results.Ok(books);
    }

    private static async Task<IResult> GetBook(int id, IBookService bookService)
    {
        var book = await bookService.GetBookAsync(id);

        if (book == null)
            return Results.NotFound();
        return Results.Ok(book);
    }

    private static async Task<IResult> CreateBook([FromBody] BookDto bookDto, IBookService bookService)
    {
        var createdBook = await bookService.AddBookAsync(bookDto);
        return Results.Created($"/api/books/{createdBook.Id}", createdBook);
    }

    private static async Task<IResult> UpdateBook(int id, [FromBody] BookDto bookDto, IBookService bookService)
    {
        var updatedBook = await bookService.UpdateBookAsync(id, bookDto);
        if (updatedBook == null)
            return Results.NotFound();
        return Results.Ok(updatedBook);
    }

    private static async Task<IResult> DeleteBook(int id, IBookService bookService)
    {
        var isDeleted = await bookService.DeleteBookAsync(id);

        if (!isDeleted)
            return Results.NotFound();
        return Results.NoContent();
    }


    private static async Task<IResult> ImportBooks(
        HttpContext context,
        [FromForm] IFormFile file,
        [FromServices] IBookService bookService,
        [FromServices] IAntiforgery antiforgery)
    {
        if (file == null || file.Length == 0)
        {
            return Results.BadRequest("Please select a csv file.");
        }

        try
        {
            var isImported = await bookService.ImportBooksFromCsvAsync(file);
            if (!isImported)
                return Results.BadRequest("Error importing books from csv file.");
            return Results.Ok("Successfully imported");
        }
        catch (Exception ex)
        {
            // Log the exception
            return Results.StatusCode(500);
        }
    }
}
