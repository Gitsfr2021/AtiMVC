using Kaspid.Models;
using Kaspid.Models.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kaspid.Areas.admin.Controllers
{
    [Authorize(Roles = "Admin,adminl2")]
    [AccessFilter]
    public class ProductGroupController : ApplicationController
    {

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        private void Iniital()
        {

            List<SelectNodeParent> categories = db.ProductGroups.AsEnumerable().Select(p => new SelectNodeParent { CategoryId = p.Id, CategoryName = p.Title, ParentId = p.ParentId, ParentCategory = null, Children = null }).ToList();
            ViewBag.Tree = SelectNodeParent.GetAllCategoriesForTreeFor(categories);
            ViewBag.Brand = db.Brands.Where(x => x.Status == (byte)AllEnums.Status.Show).Select(x => new
            {
                name = x.Title,
                value = x.Id
            }).OrderBy(x => x.name).ToList();
        }

        // GET: admin/ProductGroup
        public ActionResult Index()
        {
            this.ShowMessage();
            List<TreeNode> categories = db.ProductGroups.AsEnumerable().Where(p => p.IsDeleted == false).Select(p => new TreeNode { CategoryId = p.Id, CategoryName = p.Title, Status = p.Status, ParentId = p.ParentId, ParentCategory = null, Children = null }).ToList();
            ViewBag.Tree = TreeNode.GetAllCategoriesForTreeFor(categories,"ProductGroup");
            return View();
        }

        public ActionResult Create()
        {
            Iniital();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductGroup _ProductGroup, int? id)
        {
            if (!ModelState.IsValid)
            {

                return View(_ProductGroup);
            }
            else
            {
                try
                {
                    if (db.ProductGroups.Any(x => x.TitleURL == _ProductGroup.TitleURL))
                    {
                        ModelState.AddModelError("TitleURL","آدرس تکراری می باشد");
                        return View(_ProductGroup);
                    }
                    if (!id.HasValue || db.ProductGroups.Any(p => p.Id == id))
                    {
                        _ProductGroup.ParentId = id;
                        _ProductGroup.Id = 0;
                        _ProductGroup.Description = HttpUtility.HtmlDecode(_ProductGroup.Description);
                        db.ProductGroups.Add(_ProductGroup);
                        db.SaveChanges(Request.Path);
                        Alert("عملیات با موفقیت انجام گردید");
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View(_ProductGroup);
                    }
                }
                catch 
                {
                    return View(_ProductGroup);
                }
            }
        }
        // GET: ProductGroup/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return Redirect("/404");
            }
            ProductGroup _ProductGroup = db.ProductGroups.Find(id);
            if (_ProductGroup == null)
            {
                return Redirect("/404");
            }
            Iniital();
            return View(_ProductGroup);
        }

        // POST: ProductGroup/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductGroup _ProductGroup)
        {
            if (ModelState.IsValid)
            {
                if (db.ProductGroups.Any(x => x.TitleURL == _ProductGroup.TitleURL&&x.Id!=_ProductGroup.Id))
                {
                    ModelState.AddModelError("TitleURL","آدرس تکراری می باشد");
                    return View(_ProductGroup);
                }
                _ProductGroup.Description = HttpUtility.HtmlDecode(_ProductGroup.Description);
                db.Entry(_ProductGroup).State = EntityState.Modified;
                db.SaveChanges(Request.Path);
                Alert("عملیات با موفقیت انجام گردید");
                return RedirectToAction("Index");
            }
            Iniital();
            return View(_ProductGroup);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return Redirect("/404");
            }
            if (id==6)
            {
                Alert("این گروه ثابت است و قابل حذف نیست ");
                return RedirectToAction("Index");
            }
            ProductGroup _ProductGroup = db.ProductGroups.Find(id);

            if (_ProductGroup == null)
            {
                return Redirect("/404");
            }
            else
            {

                if (_ProductGroup.Childs.Where(x => x.IsDeleted == false).Count() > 0)
                {
                    Alert("برای حذف این شاخه باید ابتدا زیرشاخه ها را حذف نمایید");
                    return RedirectToAction("Index");
                }
                else if (_ProductGroup.Products.Count() > 0)
                {
                    Alert("برای حذف این شاخه باید ابتدا خدمات این شاخه را حذف نمایید");
                    return RedirectToAction("Index");
                }
                else
                {
                    _ProductGroup.IsDeleted = true;
                    db.Entry(_ProductGroup).State = EntityState.Modified;
                    db.SaveChanges(Request.Path);
                    Alert("عملیات با موفقیت انجام گردید");
                    return RedirectToAction("Index");
                }
            }

        }

        public ActionResult UniqeTitleURL(string TitleURL, int Id = 0)
        {
            bool UserExist = true;

            UserExist = db.ProductGroups.Any(p => p.TitleURL == TitleURL && p.IsDeleted == false && p.Id != Id);

            if (UserExist == true)
            {
                return Content("false");
            }
            else
            {
                return Content("true");
            }

        }
    }
}