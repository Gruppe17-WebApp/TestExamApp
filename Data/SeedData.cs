using TestExamApp.Models;

namespace TestExamApp.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        if (context.Libraries.Any())
        {
            return;
        }

        var library = new Library
        {
            Name = "Drammen bibliotek"
        };

        context.Libraries.Add(library);
        await context.SaveChangesAsync();

        context.Books.AddRange(
            new Book
            {
                Title = "Sult",
                Author = "Knut Hamsun",
                LibraryId = library.Id
            },
            new Book
            {
                Title = "Kristin Lavransdatter",
                Author = "Sigrid Undset",
                LibraryId = library.Id
            }
        );

        await context.SaveChangesAsync();
    }
}