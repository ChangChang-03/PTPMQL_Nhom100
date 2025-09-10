using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MVC.Models
{
    public class Person
    {
        public int PersonId { get; set; } // 👈 Primary Key
        public string FullName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int NamSinh { get; set; }
    }
}
