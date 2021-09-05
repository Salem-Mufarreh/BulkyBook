using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class LoggedInUser
    {
        [Key]
        public string Email { get; set; }
        public DateTime DateLogged { get; set; }
        public string Name { get; set; }
    }
}
