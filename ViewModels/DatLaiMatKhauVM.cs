using System.ComponentModel.DataAnnotations;

namespace FruitShopG4P.ViewModels
{
    public class DatLaiMatKhauVM
    {
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string MatKhauMoi { get; set; } = null!;

        [Required, Compare("MatKhauMoi", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        public string NhapLaiMatKhau { get; set; } = null!;
    }
}
