using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IReposaitory;
using Bulky.Models;
using Bulky.Models.View_Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitofWork unitofWork;

      
        public CompanyController(IUnitofWork unitofWork)
        {
            this.unitofWork = unitofWork;
            
        }
        public IActionResult Index()
        {
            List<Company> companies = unitofWork.Company.GetAll().ToList();
            return View(companies);
        }

        
        public IActionResult  Upsert(int? id)
        {
           

            if (id == null || id == 0)
            {
                
                return View(new Company());
            }
            else
            {
                
                Company ObjFromDb = unitofWork.Company.Get(u => u.Id==id);
                return View(ObjFromDb);
            }

        }
        [HttpPost]
        public IActionResult  Upsert(Company obj)
        {

            if (ModelState.IsValid)
            {
             
                if (obj.Id == 0)
                {
                    unitofWork.Company.Add(obj);
                }
                else
                {
                    unitofWork.Company.update(obj);
                }

                unitofWork.Save();

                TempData["Success"] = "Product Created Sucessfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
            return View(obj);
            }

        }


        
        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> objCompanyList = unitofWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }


       


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = unitofWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

          

            unitofWork.Company.Remove(CompanyToBeDeleted);
            unitofWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }

}