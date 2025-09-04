using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("Persons")]

    public class Person
    {
        [Key]

        public string FullName { get; set; } = default!;
        public int NamSinh { get; set; } = default!;
       
    }
}