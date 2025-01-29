using System.ComponentModel.DataAnnotations;

namespace BCMS_UI.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Author is required.")]
    [StringLength(100, ErrorMessage = "Author cannot exceed 100 characters.")]
    public string? Author { get; set; }

    [Required(ErrorMessage = "Genre is required.")]
    [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters.")]
    public string? Genre { get; set; }
}
