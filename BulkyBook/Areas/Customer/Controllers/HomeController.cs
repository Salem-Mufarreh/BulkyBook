using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var count = _unitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShopingCart, count);
            }
            return View(productList);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties:"Category,CoverType");
            ShoppingCart cart = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u=> u.ApplicationUserId == shoppingCart.ApplicationUserId
                    && u.ProductId == shoppingCart.ProductId,
                    includeProperties:"Product"
                    );
                if(cartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                }
                else
                {
                    cartFromDb.Count += shoppingCart.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();
                //HttpContext.Session.SetObject(SD.ssShopingCart,shoppingCart);
                HttpContext.Session.SetInt32(SD.ssShopingCart, count);
                return RedirectToAction("Index");
            }
            else
            {
                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == shoppingCart.ProductId, includeProperties: "Category,CoverType");
                ShoppingCart cart = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
            }
            
            return View(shoppingCart);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
