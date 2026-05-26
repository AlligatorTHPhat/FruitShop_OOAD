using System.ComponentModel.DataAnnotations;

namespace FruitShopG4P.ViewModels
{
    public class QuenMatKhauVM
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = null!;
    }
}
