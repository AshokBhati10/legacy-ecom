using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Core.ViewModels
{
    public class LoginViewModel
    {
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; }

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
