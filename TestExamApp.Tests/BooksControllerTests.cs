using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestExamApp.Controllers;
using TestExamApp.Data;
using TestExamApp.Models;
using TestExamApp.Services;

namespace TestExamApp.Tests;

public class BooksControllerTests
{
    [Fact]
    public async Task Index_ReturnsBooksWithLibrary()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var library = new Library
        {
            Name = "Testbibliotek",
            City = "Drammen"
        };

        context.Libraries.Add(library);
        await context.SaveChangesAsync();

        context.Books.Add(new Book
        {
            Title = "Testbok",
            Author = "Testforfatter",
            PublishedYear = 2024,
            LibraryId = library.Id
        });

        await context.SaveChangesAsync();

        var controller = new BooksController(
            context,
            new BookApiService(new HttpClient())
        );

        var result = await controller.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Book>>(viewResult.Model);
        var book = Assert.Single(model);

        Assert.Equal("Testbok", book.Title);
        Assert.NotNull(book.Library);
        Assert.Equal("Testbibliotek", book.Library.Name);
    }

    [Fact]
    public async Task Create_AddsBookToDatabase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new ApplicationDbContext(options);

        var library = new Library
        {
            Name = "Testbibliotek",
            City = "Oslo"
        };

        context.Libraries.Add(library);
        await context.SaveChangesAsync();

        var controller = new BooksController(
            context,
            new BookApiService(new HttpClient())
        );

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            PublishedYear = 2024,
            LibraryId = library.Id
        };

        var result = await controller.Create(book);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(1, await context.Books.CountAsync());
    }
}