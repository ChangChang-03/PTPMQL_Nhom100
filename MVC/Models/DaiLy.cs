using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class DaiLy
    {
        [Key]
        public string MaDaiLy { get; set; } = string.Empty;

        public string TenDaiLy { get; set; } = string.Empty;
        public string DiaChi { get; set; } = string.Empty;
        public string NguoiDaiDien { get; set; } = string.Empty;
        public string DienThoai { get; set; } = string.Empty;
    }
}
