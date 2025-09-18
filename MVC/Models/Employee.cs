using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    [Table("Employees")]
    public class Employee : Person
    {
       
        public string EmployeeId { get; set; } = string.Empty;

        public int Age { get; set; }
    }
}
