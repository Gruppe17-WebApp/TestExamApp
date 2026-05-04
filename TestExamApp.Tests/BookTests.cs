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
}