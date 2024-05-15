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
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(objProductList);
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


            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
               {
                   //property populate
                   Text = u.Name,
                   Value = u.ID.ToString()
               }) ,
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                //create
               return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
            
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM emodel, IFormFile? file)
        {
            if(ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                
                //if(_unitOfWork.Product.GetAll().Any(p => p.ISBN == emodel.Product.ISBN)){
                //    ModelState.AddModelError("ISBN", "Product with this ISBN already exist");
                //}
                //else {
                    if (file != null)
                    {
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = Path.Combine(wwwRootPath, @"images\product\");

                        if (!string.IsNullOrEmpty(emodel.Product.ImageUrl))
                        {
                            //delete the old image
                            var oldImagePath = Path.Combine(wwwRootPath, emodel.Product.ImageUrl.TrimStart('\\'));

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        emodel.Product.ImageUrl = @"\images\product\" + filename;
                    }

                    if (emodel.Product.Id == 0)
                    {
                        _unitOfWork.Product.Add(emodel.Product);
                    TempData["success"] = "Product Create Successful";
                }
                    else
                    {
                        _unitOfWork.Product.update(emodel.Product);
                    TempData["success"] = "Product Update Successful";
                }
                    _unitOfWork.Save();
                    
                    return RedirectToAction("Index");
                //}
                return View();
          }
            else
            {
                emodel.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                    {
                        //property populate
                        Text = u.Name,
                        Value = u.ID.ToString()
                    });
                return View(emodel);
            }
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new {data = objProductList});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var ProductToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if(ProductToBeDeleted == null)
            {
                return Json(new {success = false, message = "Error while deleting"});
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, ProductToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(ProductToBeDeleted);
            _unitOfWork.Save();

            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        #endregion
    }
}
