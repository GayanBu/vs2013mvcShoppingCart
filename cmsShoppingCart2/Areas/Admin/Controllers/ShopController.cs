using cmsShoppingCart2.Models.Data;
using cmsShoppingCart2.Models.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace cmsShoppingCart2.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop
        public ActionResult Categories()
        {
            List<CategoryVM> CategoryVMList;

            using (Db db = new Db())
            {
                CategoryVMList = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }
            return View(CategoryVMList);
        }

        [HttpPost]
        public ActionResult ReOrderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                CategoryDTO dto;
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
            return View();
        }
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                CategoryDTO dto = new CategoryDTO();
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                db.Categories.Add(dto);
                db.SaveChanges();

                id = dto.Id.ToString();

            }

            return id;
        }
        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);
                if (dto != null)
                {
                    db.Categories.Remove(dto);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Categories");
        }

        [HttpPost]
        public string RenameCategory(string newcatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == newcatName))
                    return "titletaken";

                CategoryDTO dto = db.Categories.Find(id);
                dto.Name = newcatName;
                dto.Slug = newcatName.Replace(" ", "-").ToLower();
                db.SaveChanges();
            }

            return "ok";
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductVM model = new ProductVM();
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                }
                return View(model);
            }

            //Make sure productname is unique
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That product name id taken!");
                    return View(model);
                }
            }
            int id;
            using (Db db = new Db())
            {
                ProductDTO productdto = new ProductDTO();
                productdto.Name = model.Name;
                productdto.Slug = model.Name.Replace(" ", "-").ToLower();
                productdto.Description = model.Description;
                productdto.Price = model.Price;
                productdto.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                productdto.CategoryName = catDTO.Name;

                db.Products.Add(productdto);
                db.SaveChanges();

                id = productdto.Id;
            }
            //set tempdata message
            TempData["SM"] = "You have added a product!";

            #region Upload image

            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);
            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);
            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);
            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);
            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded or wrong image extension");
                    }
                }
                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }

            #endregion
            return RedirectToAction("AddProduct");
        }
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            List<ProductVM> listOfProductVM = new List<ProductVM>();

            var pageNumber = page ?? 1;
            using (Db db = new Db())
            {
                listOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                ViewBag.SelectedCat = catId.ToString();

                var OnePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
                ViewBag.OnePageOfProducts = OnePageOfProducts;
            }

            return View();
        }
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            ProductVM model;
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                if (dto == null)
                {
                    return Content("That product does not exists");
                }
                model = new ProductVM(dto);

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs")).Select(fn => Path.GetFileName(fn));
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            int id = model.Id;
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs")).Select(fn => Path.GetFileName(fn));
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Products name is taken!");
                    return View(model);
                }
            }
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;
                db.SaveChanges();
            }
            #region ImageUpload
            if (file != null && file.ContentLength > 0)
            {
                string ext = file.ContentType.ToLower();

                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded or wrong image extension");
                    }
                }
                var originalDirectory = new DirectoryInfo(String.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                {
                    file2.Delete();
                }
                foreach (FileInfo file3 in di2.GetFiles())
                {
                    file3.Delete();
                }
                string imgName = file.FileName;
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imgName;
                    db.SaveChanges();
                }
                var path = string.Format("{0}\\{1}", pathString1, imgName);
                var path2 = string.Format("{0}\\{1}", pathString2, imgName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }
            #endregion

            TempData["SM"] = "You have edited the product!";

            return RedirectToAction("EditProduct");
        }
        public ActionResult DeleteProduct(int id)
        {
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);

                db.SaveChanges();
            }
            var originalDirectory = new DirectoryInfo(String.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            if (Directory.Exists(pathString))
            {
                Directory.Delete(pathString,true);
            }
            return RedirectToAction("Products");
        }
    }
}