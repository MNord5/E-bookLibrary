using E_bookLibrary.Data;
using E_bookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_bookLibrary.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly AppDbContext _db;

        public LibraryController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Book> objBookList = _db.Ebooks;
            return View(objBookList);
        }
        public IActionResult Profile()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(EbookUpload addBook)
        {
            if (ModelState.IsValid)
            {
                
                var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                
                
                using (var memoryStream = new MemoryStream())
                {
                    await addBook.FormFile.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 2097152)
                    {
                        var ebook = new Book()
                        {
                            Title = addBook.Title,
                            Genre = addBook.Genre,
                            Author = addBook.Author,
                            EbookFile = memoryStream.ToArray(),
                            UserId = int.Parse(userId)
                        };
                        
                        _db.Ebooks.Add(ebook);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
            }
            return View();
        }

    }
}
