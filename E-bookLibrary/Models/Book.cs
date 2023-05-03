using System.ComponentModel.DataAnnotations;

namespace E_bookLibrary.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;


        public byte[]? EbookFile { get; set; }
        public string ImgUrl { get; set; } = string.Empty;

        public int UserId { get; set; }
        public UserModel User { get; set; }

    }
}
