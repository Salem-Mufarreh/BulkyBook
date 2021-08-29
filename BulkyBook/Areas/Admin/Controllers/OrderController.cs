using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
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
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            orderHeader.OrderStatus = SD.StatusInProcess;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.ShippingDate = DateTime.Now;
            orderHeader.OrderStatus = SD.StatusShipped;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            if(orderHeader.PaymentStatus == SD.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.TransactionId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                orderHeader.OrderStatus = SD.StatusRefunded;
                orderHeader.PaymentStatus = SD.StatusRefunded;
            }
            else
            {
                orderHeader.OrderStatus = SD.StatusCancelled;
                orderHeader.PaymentStatus = SD.StatusCancelled;
            }
            
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(nameof(Details))]
        public IActionResult Details(string stripeToken)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            if(stripeToken != null)
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order ID : " + orderHeader.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.BalanceTransactionId == null)
                {
                    orderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
                else
                {
                    orderHeader.TransactionId = charge.BalanceTransactionId;
                }
                if (charge.Status.ToLower() == "succeeded")
                {
                    orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    orderHeader.PaymentDate = DateTime.Now;
                }
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Details), "Order", new { id = orderHeader.Id });
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
