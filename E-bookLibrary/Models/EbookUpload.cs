using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_bookLibrary.Models
{
    public class EbookUpload
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

       
        [DisplayName("Epub file")]
        public IFormFile FormFile { get; set; }
    }
}
