using System.ComponentModel.DataAnnotations;

namespace TestExamApp.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Author { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Isbn { get; set; }

    public int PublishedYear { get; set; }

    [Required]
    public int LibraryId { get; set; }

    public Library? Library { get; set; }
}