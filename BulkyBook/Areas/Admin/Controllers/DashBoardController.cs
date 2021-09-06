using BulkyBook.DataAccess.Repository;
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
    [Authorize(Roles = SD.Role_Admin)]
    public class DashBoardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashBoardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            DashBoardVM dashBoardVM = new DashBoardVM();
            var sales = 0.0;
            var total = 0.0;
            var orderList = _unitOfWork.OrderHeader.GetAll(u => u.OrderDate >= DateTime.Today).ToList();
            foreach (var item in orderList)
            {
                sales += item.OrderTotal;
            }
            var list = _unitOfWork.OrderHeader.GetAll();
            foreach(var item in list)
            {
                total += item.OrderTotal;
            }
            var loggedUser = _unitOfWork.loggedUsers.GetAll().ToList();

            dashBoardVM.TotalAmount = total;
            dashBoardVM.Clients = list.Count();
            dashBoardVM.Sales = sales;
            dashBoardVM.loggedInUser = loggedUser;
            dashBoardVM.orderHeader = _unitOfWork.OrderHeader.GetAll().ToList();
            return View(dashBoardVM);
        }


        #region API CALL
        [HttpGet]
        public IActionResult GetData()
        {
            var data = _unitOfWork.OrderHeader.GetAll().GroupBy(a => new { a.OrderDate.Month})
                .Select(g => new { OrderDate = g.Key, total = g.Sum(a => a.OrderTotal) }).ToList();
            List<double> total = new List<double>();
            List<int> month = new List<int>();
            foreach(var item in data)
            {
                total.Add(item.total);
                month.Add(item.OrderDate.Month);
            }

            return Json(new { date=month, amount=total });
        }
        #endregion

    }

}
