using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}
