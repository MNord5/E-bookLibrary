
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace E_bookLibrary.Models
{
    public class AppUser
    {
        [Required, MinLength(3)]
        [DisplayName("User Name")]
        public string UserName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}
