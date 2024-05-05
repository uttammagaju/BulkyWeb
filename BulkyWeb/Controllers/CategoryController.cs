using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepo;
        public CategoryController(ICategoryRepository db)
        {
            _categoryRepo = db;
        }
        public IActionResult Index()
        {
           List<Category> objCategoryList = _categoryRepo.GetAll().ToList();
            return View(objCategoryList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category emodel)
        {
            if(emodel.Name == emodel.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name","The DisplayOrder cannot exactly match the Name.");
            }
            if (emodel.Name == "test")
            {
               ModelState.AddModelError("name", "Test is an invalid value");
            }
            if (ModelState.IsValid)
            {
                _categoryRepo.Add(emodel);
                _categoryRepo.Save();
                TempData["success"] = "Category Create Sucessfully";
                return RedirectToAction("Index");
            }
            
            return View(emodel);
           
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(id == 0 || id == null)
            {
                return NotFound();
            }
            Category? categoryFromDb = _categoryRepo.Get(u => u.ID == id);
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category emodel)
        {
            if (ModelState.IsValid)
            {
                _categoryRepo.Update(emodel);
                _categoryRepo.Save();
                TempData["success"] = "Category Update Sucessfully";
            }
            if (emodel == null)
            {
                return NotFound(ModelState);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if(id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _categoryRepo.Get(u => u.ID == id);
            return View(categoryFromDb);      
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(Category emodel)//(int? id)
        {
            //Category? obj = _categoryRepo.Get(u => u.ID == id)
            if (ModelState.IsValid)
            {
                _categoryRepo.Remove(emodel);
                _categoryRepo.Save();
                TempData["success"] = "Category Delete Sucessfully";
                Console.WriteLine(TempData["sucess"]);
            }
            if(emodel == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }

    }
}
