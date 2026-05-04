using System.Net.Http;
using System.Text.Json;
using TestExamApp.Models;

namespace TestExamApp.Services
{
    public class BookApiService
    {
        private readonly HttpClient _httpClient;

        public BookApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Book>> GetBooksFromApi()
        {
            var url = "https://openlibrary.org/search.json?q=programming";

            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            var books = new List<Book>();

            var docs = doc.RootElement.GetProperty("docs");

            foreach (var item in docs.EnumerateArray().Take(5))
            {
                var title = item.GetProperty("title").GetString();

                string author = "Ukjent";
                if (item.TryGetProperty("author_name", out var authors))
                {
                    author = authors[0].GetString() ?? "Ukjent";
                }

                books.Add(new Book
                {
                    Title = title ?? "Ukjent",
                    Author = author,
                    PublishedYear = 2000,
                    LibraryId = 1
                });
            }

            return books;
        }
    }
}