using cmsShoppingCart2.Models.Data;
using cmsShoppingCart2.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cmsShoppingCart2.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PagesVM> PagesList;
            using (Db db = new Db())
            {
                PagesList = db.Pages.ToArray().OrderBy(s => s.Sorting).Select(x => new PagesVM(x)).ToList();
            }

            return View(PagesList);
        }
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddPage(PagesVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                string slug;
                PageDTO dto = new PageDTO();
                dto.Title = model.Title;
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == model.Slug))
                {
                    ModelState.AddModelError("", "That Title or Slug already exists");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;

                db.Pages.Add(dto);
                db.SaveChanges();
            }

            TempData["SM"] = "You have added a page!";
            return RedirectToAction("AddPage");
        }

        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PagesVM model;
            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);
                if (dto == null)
                {
                    return Content("The page does not exist");
                }
                model = new PagesVM(dto);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditPage(PagesVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                int id = model.Id;
                string slug = "";
                PageDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-");
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-");
                    }

                }
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) || db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That Title or Slug already exists");
                    return View(model);
                }

                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;

                db.SaveChanges();
            }

            TempData["SM"] = "You have edited the page!";
            return RedirectToAction("EditPage");
        }
        [HttpGet]
        public ActionResult PageDetails(int id)
        {
            PagesVM model;
            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);
                if (dto == null)
                {
                    return Content("Page does not exists!");
                }
                model = new PagesVM(dto);
            }
            return View(model);
        }

        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                PageDTO dto = db.Pages.Find(id);
                if (dto != null)
                {
                    db.Pages.Remove(dto);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult ReOrderPages(int[] id)
        {
            using (Db db = new Db())
            {
                int count = 1;
                PageDTO dto;
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EditSideBar()
        {
            SideBarVM model;
            using (Db db = new Db())
            {
                SideBarDTO dto = db.SideBar.Find(1);
                model = new SideBarVM(dto);
            }
            return View();
        }

        [HttpPost]
        public ActionResult EditSideBar(SideBarVM model)
        {
            using (Db db = new Db())
            {
                SideBarDTO dto = db.SideBar.Find(1);
                dto.Body = model.Body;
                db.SaveChanges();
            }
            TempData["SM"] = "You have edited the sidebar";
            return RedirectToAction("EditSidebar");
        }
    }

}