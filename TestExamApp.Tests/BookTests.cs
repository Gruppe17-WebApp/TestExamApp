using System.ComponentModel.DataAnnotations;
using TestExamApp.Models;

namespace TestExamApp.Tests;

public class BookTests
{
    [Fact]
    public void Book_CanBeCreated_WithValidValues()
    {
        var book = new Book
        {
            Title = "Clean Code",
            Author = "Robert C. Martin",
            Isbn = "9780132350884",
            PublishedYear = 2008,
            LibraryId = 1
        };

        Assert.Equal("Clean Code", book.Title);
        Assert.Equal("Robert C. Martin", book.Author);
        Assert.Equal(2008, book.PublishedYear);
        Assert.Equal(1, book.LibraryId);
    }

    [Fact]
    public void Library_CanContainManyBooks()
    {
        var library = new Library
        {
            Name = "Vestfold bibliotek",
            City = "Horten",
            Books = new List<Book>
            {
                new Book { Title = "Book 1", Author = "Author 1", LibraryId = 1 },
                new Book { Title = "Book 2", Author = "Author 2", LibraryId = 1 }
            }
        };

        Assert.Equal(2, library.Books.Count);
    }

    [Fact]
    public void Book_WithoutTitle_IsInvalid()
    {
        var book = new Book
        {
            Title = "",
            Author = "Knut Hamsun",
            PublishedYear = 1890,
            LibraryId = 1
        };

        var results = ValidateModel(book);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Book.Title)));
    }

    [Fact]
    public void Library_WithoutName_IsInvalid()
    {
        var library = new Library
        {
            Name = "",
            City = "Drammen"
        };

        var results = ValidateModel(library);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Library.Name)));
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);

        Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        return results;
    }
}