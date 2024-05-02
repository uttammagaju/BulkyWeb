using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
           List<Category> objCategoryList = _db.Categories.ToList();
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
                _db.Categories.Add(emodel);
                _db.SaveChanges();
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
            Category? categoryFromDb = _db.Categories.Find(id);
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
                _db.Categories.Update(emodel);
                _db.SaveChanges();
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
            Category? categoryFromDb = _db.Categories.Find(id);
            return View(categoryFromDb);      
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(Category emodel)
        {
            if (ModelState.IsValid)
            {
                _db.Remove(emodel);
                _db.SaveChanges();
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
