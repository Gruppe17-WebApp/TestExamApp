using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; 
using TestExamApp.Data;
using TestExamApp.Models;
using TestExamApp.Services;

namespace TestExamApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BookApiService _apiService;

        public BooksController(ApplicationDbContext context, BookApiService apiService)
        {
            _context = context;
            _apiService = apiService;
        }

        [AllowAnonymous] 
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Books.Include(b => b.Library);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize] 
        public async Task<IActionResult> ImportFromApi()
        {
            var defaultLibrary = await _context.Libraries.FirstOrDefaultAsync();

            if (defaultLibrary == null)
            {
                defaultLibrary = new Library
                {
                    Name = "Importert bibliotek",
                    City = "Ukjent"
                };

                _context.Libraries.Add(defaultLibrary);
                await _context.SaveChangesAsync();
            }

            var booksFromApi = await _apiService.GetBooksFromApi();

            foreach (var book in booksFromApi)
            {
                if (!_context.Books.Any(b => b.Title == book.Title))
                {
                    book.LibraryId = defaultLibrary.Id;
                    _context.Books.Add(book);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Details/5
        [AllowAnonymous] 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Library)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize] 
        public IActionResult Create()
        {
            ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Name");
            return View();
        }

        [Authorize] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Isbn,PublishedYear,LibraryId")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Name", book.LibraryId);
            return View(book);
        }

        [Authorize] 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Name", book.LibraryId);
            return View(book);
        }

        [Authorize] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Isbn,PublishedYear,LibraryId")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["LibraryId"] = new SelectList(_context.Libraries, "Id", "Name", book.LibraryId);
            return View(book);
        }

        [Authorize] 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Library)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [Authorize] 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}