using E_bookLibrary.Data;
using E_bookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Drawing;
using System.IO;
using VersOne.Epub;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IActionResult> Index(string searchString)
        {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var id = int.Parse(userId);

            var bookList = from b in _db.Ebooks
                .Where(b => b.UserId.ToString().Contains(userId)) select b;
            /*
            IEnumerable < Book > objBookList = await _db.Ebooks
                .Where(b => b.UserId.ToString().Contains(userId)).ToListAsync();
            */
            if (!String.IsNullOrEmpty(searchString))
            {
                bookList = bookList.Where(s => s.Title!.Contains(searchString));
            }

            return View(await bookList.ToListAsync());
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
                        TempData["success"] = "Ebook uploaded succesfully!";
                    }
                    else
                    {
                        ModelState.AddModelError("File", "The file is too large.");
                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ebook = await _db.Ebooks
                .FirstOrDefaultAsync(b => b.Id == id);
            if (ebook == null)
            {
                return NotFound();
            }

            return View(ebook);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ebook = await _db.Ebooks.FindAsync(id);
            _db.Ebooks.Remove(ebook);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Read()
        {
            /*
            var testBook = _db.Ebooks.First();
            System.IO.File.WriteAllBytes("test.epub", testBook.EbookFile);
            EpubBook epubBook = EpubReader.ReadBook("test.epub");
            var ebook = new EbookUpload();
            ebook.Title = epubBook.Title;
            ebook.Author = epubBook.Author;
            return View(ebook);
            */
            return View();
        }
    }
}
