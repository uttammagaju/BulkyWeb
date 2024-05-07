using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
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
            if (emodel.Name == emodel.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }
            if (emodel.Name == "test")
            {
                ModelState.AddModelError("name", "Test is an invalid value");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(emodel);
                _unitOfWork.Save();
                TempData["success"] = "Category Create Successfully";
                return RedirectToAction("Index");
            }

            return View(emodel);

        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.ID == id);
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
                _unitOfWork.Category.Update(emodel);
                _unitOfWork.Save();
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
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.ID == id);
            return View(categoryFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(Category emodel)//(int? id)
        {
            //Category? obj = _categoryRepo.Get(u => u.ID == id)
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Remove(emodel);
                _unitOfWork.Save();
                TempData["success"] = "Category Delete Sucessfully";
                Console.WriteLine(TempData["sucess"]);
            }
            if (emodel == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index");
        }

    }
}
