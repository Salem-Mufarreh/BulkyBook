using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderDetailsVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Details(int id)
        {
            OrderVM = new OrderDetailsVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == id, includeProperties: "Product")
            };
            return View(OrderVM);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<OrderHeader> ordersList;
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                 ordersList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            }
            else
            {
                ordersList = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    ordersList = ordersList.Where(o => o.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    ordersList = ordersList.Where(o => o.OrderStatus == SD.StatusInProcess 
                                                    || o.OrderStatus == SD.StatusApproved
                                                    || o.OrderStatus == SD.StatusPending);

                    break;
                case "rejected":
                    ordersList = ordersList.Where(o => o.OrderStatus == SD.StatusCancelled
                                                    || o.OrderStatus == SD.StatusRefunded
                                                    || o.OrderStatus == SD.PaymentStatusRejected);

                    break;
                case "completed":
                    ordersList = ordersList.Where(o => o.OrderStatus == SD.StatusShipped);

                    break;
                default:
                    
                    break;


            }
            return Json(new { data = ordersList });
        }
        #endregion


    }

}
