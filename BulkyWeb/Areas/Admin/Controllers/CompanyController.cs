using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
            //   .GetAll().Select(u => new SelectListItem
            //   {
            //       //property populate
            //       Text = u.Name,
            //       Value = u.ID.ToString()
            //   });

            //ViewBag.CategoryList = CategoryList;
            //Alternative 
            //ViewData["CategoryList"] = CategoryList;


           
            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company companyObj= _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }

        }
        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {
                if (companyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(companyObj);
                    TempData["success"] = "Company Create Successful";
                }
                else
                {
                    _unitOfWork.Company.Update(companyObj);
                    TempData["success"] = "Company Update Successful";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
               
            }
            else
            {
                 return View(companyObj);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

           
            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();
            return Json(new {success = true, message="Delete Successful" });
        }
        #endregion
    }
}
