using System.Drawing;

namespace E_bookLibrary.Models
{
    public class ReadBook
    {
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string imgcover { get; set; } = string.Empty;
        public List<VersOne.Epub.EpubNavigationItem>? Navigation { get; set; }
        public List<VersOne.Epub.EpubTextContentFile>? Content { get; set; }


    }
}
