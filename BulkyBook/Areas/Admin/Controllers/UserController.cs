using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utility;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
       

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {

            var userList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach(var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if(user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new { data = userList });
        }
        [HttpPost]
        public IActionResult PostRows()
        {
            int start = Convert.ToInt32(Request.Form["start"]);
            int length = Convert.ToInt32(Request.Form["length"]);
            string searchValue = Request.Form["search[value]"];
            string sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][name]"];
            string sortDirection = Request.Form["order[0][dir]"];

            var userList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            if (!string.IsNullOrEmpty(searchValue))//filter
            {
                userList = userList.
                    Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) || x.Email.ToLower().Contains(searchValue.ToLower()) || x.PhoneNumber.ToLower().Contains(searchValue.ToLower()) || x.Company.ToString().Contains(searchValue.ToLower()) || x.Role.ToString().Contains(searchValue.ToLower())).ToList<ApplicationUser>();
            }
            int totalrowsafterfiltering = userList.Count;
            //sorting
            
                ///.OrderBy<IEnumerable<string>,string>(sortColumnName + " " + sortDirection).ToList<ApplicationUser>();

            //paging
            userList = userList.Skip(start).Take(length).ToList<ApplicationUser>();


            return Json(new { data = userList, draw = Request.Form["draw"], recordsTotal = userList.Count(), recordsFiltered = totalrowsafterfiltering });
        }
            //[HttpDelete]
            //public IActionResult Delete(int id)
            //{
            //    var objFromDb = _unitOfWork.Category.Get(id);
            //    if(objFromDb == null)
            //    {
            //        return Json(new { success = false, message = "Error While Deleting" });
            //    }
            //    _unitOfWork.Category.Remove(objFromDb);
            //    _unitOfWork.Save();
            //    return Json(new { success = true, message = "Delete Successful" });
            //}

            [HttpPost]
        public IActionResult LockUnlock ([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if(objFromDb == null)
            {
                return Json( new { success = false, message="Error While Locking/Unlocking" });
            }
            if(objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user currently locked
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddDays(3);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful." });
        }

        #endregion
    }
}
