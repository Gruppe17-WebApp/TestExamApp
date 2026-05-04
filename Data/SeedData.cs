using TestExamApp.Models;

namespace TestExamApp.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        if (context.Books.Any())
        {
            return;
        }

        var library = context.Libraries.FirstOrDefault();

        if (library == null)
        {
            library = new Library
            {
                Name = "Drammen bibliotek",
                City = "Drammen"
            };

            context.Libraries.Add(library);
            await context.SaveChangesAsync();
        }

        context.Books.AddRange(
            new Book
            {
                Title = "Sult",
                Author = "Knut Hamsun",
                Isbn = "9788205278849",
                PublishedYear = 1890,
                LibraryId = library.Id
            },
            new Book
            {
                Title = "Kristin Lavransdatter",
                Author = "Sigrid Undset",
                Isbn = "9788203353067",
                PublishedYear = 1920,
                LibraryId = library.Id
            },
            new Book
            {
                Title = "Naiv. Super.",
                Author = "Erlend Loe",
                Isbn = "9788202301458",
                PublishedYear = 1996,
                LibraryId = library.Id
            }
        );

        await context.SaveChangesAsync();
    }
}