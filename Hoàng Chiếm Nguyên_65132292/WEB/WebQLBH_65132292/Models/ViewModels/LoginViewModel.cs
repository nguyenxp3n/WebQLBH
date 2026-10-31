using System.ComponentModel.DataAnnotations;

namespace DbQLBH_65132292.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui long nhap ten dang nhap.")]
        [StringLength(64)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui long nhap mat khau.")]
        [DataType(DataType.Password)]
        [StringLength(128)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
