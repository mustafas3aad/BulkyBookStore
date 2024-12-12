using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IReposaitory;
using Bulky.Models;
using Bulky.Models.View_Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitofWork unitofWork;

        public IWebHostEnvironment WebHostEnvironment { get; }
                                                       
        public ProductController(IUnitofWork unitofWork,IWebHostEnvironment webHostEnvironment)
        {
            this.unitofWork = unitofWork;
            WebHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> products = unitofWork.product.GetAll(includeProperties:"Category").ToList();
            return View(products);
        }

       
        public IActionResult  Upsert(int? id)
        {
            
             
            ProductVM productVM = new()
            {
                
                CategoryList = unitofWork.Category.GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }),

                Product =new Product()
            };

          

            if (id == null || id == 0)
            {
                
                return View(productVM);
            }
            else
            {
                productVM.Product=unitofWork.product.Get(u=>u.Id == id,includeProperties:"ProductImages");
                return View(productVM);
            }
  
        }
        [HttpPost]
        public IActionResult  Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
             

                if (productVM.Product.Id == 0)
                {
                    unitofWork.product.Add(productVM.Product);
                }
                else
                {
                    unitofWork.product.update(productVM.Product);
                }
                unitofWork.Save();


                string wwwRootPath = WebHostEnvironment.WebRootPath;
                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                            Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);

                    }
                 

                    unitofWork.product.update(productVM.Product);
                    unitofWork.Save();




                }


     

                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {

                productVM.CategoryList = unitofWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

                return View(productVM);
            }
        }

       

        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = unitofWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(WebHostEnvironment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                unitofWork.ProductImage.Remove(imageToBeDeleted);
                unitofWork.Save();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });
        }


        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = unitofWork.product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }


        
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted =unitofWork.product.Get(u=>u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new {success = false,message = "Error while deleting"});
            }


            
            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(WebHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }




           
            unitofWork.product.Remove(productToBeDeleted);
            unitofWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }

}
