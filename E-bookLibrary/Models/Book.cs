using System.ComponentModel.DataAnnotations;

namespace E_bookLibrary.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Genre { get; set; }
        [Required]
        public string Author { get; set; }
        public byte[] EbookFile { get; set; }
        public string ImgUrl { get; set; } 

    }
}
