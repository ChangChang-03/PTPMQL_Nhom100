using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MVC.Models
{
    [Table("Student")]
    public class Student
    {
        [Key]
        public string MaSinhVien { get; set; } = string.Empty;
        [Required]
        public string FullName { get; set; } 
    }
}