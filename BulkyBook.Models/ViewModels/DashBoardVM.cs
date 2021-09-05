using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class DashBoardVM
    {
        public double TotalAmount { get; set; }
        public int Clients { get; set; }
        public double Sales { get; set; }
        public List<LoggedInUser> loggedInUser { get; set; }
        public List<OrderHeader> orderHeader { get; set; }
    }
}
