using Braintree;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class BrainTreeController : Controller
    {
        public IBrainTreeGate _brain { get; set; }
        public BrainTreeController(IBrainTreeGate brain)
        {
            _brain = brain;
        }

        public IActionResult Index()
        {
            var gateway = _brain.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.clientToken = clientToken;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(IFormCollection collection) 
        {
            Random rmd = new Random();
            string nonceFromClient = collection["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = rmd.Next(1, 100),
                PaymentMethodNonce = nonceFromClient,
                OrderId = "1245",
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }

            };
            var gateway = _brain.GetGateway();
            Result<Transaction> result = gateway.Transaction.Sale(request);
            
            if(result.Target.ProcessorResponseText == "Approved")
            {
                TempData["Success"] = "Transaction Was Successful Transaction ID:" + result.Target.Id
                    + ", Amount Charged " + result.Target.Amount;
            }
           
            return RedirectToAction(nameof(Index));
        }
    }
}
