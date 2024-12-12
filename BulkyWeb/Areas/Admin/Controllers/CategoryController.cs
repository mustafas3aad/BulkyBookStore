using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IReposaitory;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    

    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitofWork unitofWork;

        public CategoryController(IUnitofWork unitofWork)
        {
            this.unitofWork = unitofWork;
        }
        public IActionResult Index()
        {
            List<Category> categories = unitofWork.Category.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            
            if (ModelState.IsValid)
            {
                unitofWork.Category.Add(obj);
                unitofWork.Save();

                

                TempData["Success"] = "Category Created Sucessfully";
                return RedirectToAction(nameof(Index));
            }
            return View();

        }
        public IActionResult Edit(int? id)
        {
            
            if (id == null || id == 0)
                return NotFound();

            Category? CategoryFromDb = unitofWork.Category.Get(x => x.Id == id);
            
            if (CategoryFromDb == null)
                return NotFound();

            return View(CategoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                unitofWork.Category.update(obj);
                unitofWork.Save();
                TempData["Success"] = "Category Update Sucessfully";
                return RedirectToAction(nameof(Index));
            }
            return View();

        }

        public IActionResult Delete(int? id)
        {
            
            if (id == null || id == 0)
                return NotFound();

            Category? CategoryFromDb = unitofWork.Category.Get(x => x.Id == id);

            
            if (CategoryFromDb == null)
                return NotFound();

            return View(CategoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? CategoryFromDb = unitofWork.Category.Get(x => x.Id == id);

            
            if (CategoryFromDb == null)
                return NotFound();

            unitofWork.Category.Remove(CategoryFromDb);
            unitofWork.Save();
            TempData["Success"] = "Category Deleted Sucessfully";
            return RedirectToAction(nameof(Index));


        }
    }
}
