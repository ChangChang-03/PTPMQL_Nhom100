using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("HeThongPhanPhoi")]
    public class HeThongPhanPhoi
    {
        [Key]
        [StringLength(20)]
        public string MaHTPP { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string TenHTPP { get; set; } = string.Empty;
        
    }
}

