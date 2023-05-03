using E_bookLibrary.Data;
using E_bookLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Drawing;
using System.IO;
using VersOne.Epub;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Text;
using Microsoft.AspNetCore.StaticFiles;

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

        public async Task<IActionResult> Index(string searchString, int currentPage=1)
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

            var totalBooks = bookList.Count();
            int pageSize = 5;
            int totalPages = (int)Math.Ceiling((decimal)totalBooks / (decimal)pageSize);
            bookList = bookList.Skip((currentPage - 1) * pageSize).Take(pageSize);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = currentPage;
            ViewBag.PageSize = pageSize;

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

                string type = ".epub";
                string filePath = Path.GetExtension(addBook.FormFile.FileName);
                bool correctExtension = IsValidExtension(filePath, type);

                
                if (!correctExtension)
                {
                    ModelState.AddModelError("FormFile", "Wrong file format.");

                }

                using (var memoryStream = new MemoryStream())
                {

                    await addBook.FormFile.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 2097152 && correctExtension)
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
                        TempData["success"] = "Book added succesfully!";
                    }
                    else
                    {

                        ModelState.AddModelError("FormFile", "File is too large.");
                    }
                }
            }
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Book editBook)
        {

            
            
                var ebook = await _db.Ebooks.FindAsync(editBook.Id);
                ebook.Author = editBook.Author;
                ebook.Title = editBook.Title;

                _db.Ebooks.Update(ebook);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            

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
        public async Task<IActionResult> Read(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var testBook = await _db.Ebooks.FirstOrDefaultAsync(b => b.Id == id);
            if(testBook == null)
            {
                return NotFound();
            }
            System.IO.File.WriteAllBytes("test.epub", testBook.EbookFile);
            EpubBook epubBook = EpubReader.ReadBook("test.epub");
            var ebook = new ReadBook();
            ebook.Title = epubBook.Title;
            ebook.Author = epubBook.Author;
            ebook.Description = epubBook.Description;
            ebook.Navigation = epubBook.Navigation;
            ebook.Content = epubBook.ReadingOrder;

            if (epubBook.CoverImage != null)
            {
                using (MemoryStream coverImageStream = new MemoryStream(epubBook.CoverImage))
                {
                    Image coverImage = Image.FromStream(coverImageStream);
                    byte[] bytes = (byte[])(new ImageConverter()).ConvertTo(coverImage, typeof(byte[]));
                    ebook.imgcover = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(bytes));
                }
            }
            
            return View(ebook);
           
           
        }

        public static bool IsValidExtension(string filepath, string desiredType)
        {
            return Path.GetExtension(filepath) == desiredType;
        }

        public string GetMimeType(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }
    }
}
