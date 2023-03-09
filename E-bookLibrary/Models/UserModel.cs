using System.ComponentModel.DataAnnotations;

namespace E_bookLibrary.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public string? VerificationToken { get; set; } 
        public DateTime? VerifiedAt { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public List<Book> Ebooks { get; set; }
    }
}
