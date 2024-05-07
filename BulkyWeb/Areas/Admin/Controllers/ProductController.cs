using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();

            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
                .GetAll().Select(u => new SelectListItem
            {
                    //property populate
                    Text = u.Name,
                    Value = u.ID.ToString()
            });
            return View(objProductList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product emodel)
        {
            if(ModelState.IsValid)
            {
                if(_unitOfWork.Product.GetAll().Any(p => p.ISBN == emodel.ISBN)){
                    ModelState.AddModelError("ISBN", "Product with this ISBN already exist");
                }
                else {
                    _unitOfWork.Product.Add(emodel);
                    _unitOfWork.Save();
                    TempData["success"] = "Product Create Successful";
                    return RedirectToAction("Index");
                }
                
            }
            return View(emodel);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            if(productFromDb == null)
            {
                return NotFound();
            }
           
            return View(productFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Product emodel)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.Product.update(emodel);
                _unitOfWork.Save();
                TempData["success"] = "Product Update Successfully";
            }
            if(emodel == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            Product? productFromDb = _unitOfWork.Product.Get(u => u.Id == id);
            return View(productFromDb);
        }
        [HttpPost]
        public IActionResult Delete(Product emodel)
        {
            if(ModelState.IsValid) {
                _unitOfWork.Product.Remove(emodel);
                _unitOfWork.Save();
                TempData["success"] = "Product Delect Successfully";
            }
            if(emodel == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }
    }
}
