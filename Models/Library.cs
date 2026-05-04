using System.ComponentModel.DataAnnotations;

namespace TestExamApp.Models;

public class Library
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    public List<Book> Books { get; set; } = new();
}