using System.Net;
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
            var query = WebUtility.UrlEncode("norsk roman");
            var url = $"https://bibliografisk.bs.no/v1/works?query={query}&limit=10";

            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            var books = new List<Book>();

            var works = doc.RootElement.GetProperty("works");

            foreach (var work in works.EnumerateArray())
            {
                var title = GetString(work, "name");
                var author = GetAuthor(work);
                var publication = GetFirstPublication(work);

                if (string.IsNullOrWhiteSpace(title) || publication.ValueKind == JsonValueKind.Undefined)
                {
                    continue;
                }

                var isbn = GetString(publication, "isbn");
                var publishedYear = ParseYear(GetString(publication, "datePublished"));

                books.Add(new Book
                {
                    Title = title,
                    Author = author,
                    Isbn = isbn,
                    PublishedYear = publishedYear
                });
            }

            return books;
        }

        private static string GetAuthor(JsonElement work)
        {
            if (!work.TryGetProperty("creator", out var creators) || creators.GetArrayLength() == 0)
            {
                return "Ukjent forfatter";
            }

            var firstCreator = creators[0];
            var name = GetString(firstCreator, "name");

            return string.IsNullOrWhiteSpace(name) ? "Forfatter ikke oppgitt" : name;
        }

        private static JsonElement GetFirstPublication(JsonElement work)
        {
            if (!work.TryGetProperty("publications", out var publications) || publications.GetArrayLength() == 0)
            {
                return default;
            }

            return publications[0];
        }

        private static string? GetString(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property))
            {
                return null;
            }

            return property.ValueKind == JsonValueKind.String ? property.GetString() : null;
        }

        private static int ParseYear(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 2000;
            }

            var digits = new string(value.Where(char.IsDigit).Take(4).ToArray());

            return int.TryParse(digits, out var year) ? year : 2000;
        }
    }
}