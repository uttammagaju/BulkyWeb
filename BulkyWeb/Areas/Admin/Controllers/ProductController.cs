using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
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
                
                if(_unitOfWork.Product.GetAll().Any(p => p.ISBN == emodel.Product.ISBN)){
                    ModelState.AddModelError("ISBN", "Product with this ISBN already exist");
                }
                else {
                    if (file != null)
                    {
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = Path.Combine(wwwRootPath, @"images\product\");
                        using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        emodel.Product.ImageUrl = @"\images\product\" + filename;
                       
                    }
                    _unitOfWork.Product.Add(emodel.Product);
                    _unitOfWork.Save();
                    TempData["success"] = "Product Create Successful";
                    return RedirectToAction("Index");
                }
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
