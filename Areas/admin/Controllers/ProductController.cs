using Kaspid.Models;
using Kaspid.Models.Repository;
using Kaspid.Models.Utility;
using Kaspid.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;


namespace Kaspid.Areas.admin.Controllers
{
    [Authorize(Roles = "Admin,adminl2")]
    [AccessFilter]
    public class ProductController : ApplicationController
    {
        private void Iniital()
        {
            List<SelectNode> categories = db.ProductGroups.Where(p => p.IsDeleted == false && p.Status == (byte)AllEnums.Status.Show).AsEnumerable().Select(p => new SelectNode { CategoryId = p.Id, CategoryName = p.Title, ParentId = p.ParentId, ParentCategory = null, Children = null }).ToList();
            string tree_result = SelectNode.GetAllCategoriesForTreeFor(categories);
            ViewBag.Tree = tree_result;

            ViewBag.Brand = db.Brands.Where(x => x.Status == (byte)AllEnums.Status.Show && x.IsDeleted==false).Select(x => new
            {
                name = x.Title,
                value = x.Id
            }).OrderBy(x => x.name).ToList();

            ViewBag.type = db.SendTypes.AsEnumerable().Where(x => x.Status == (byte)AllEnums.Status.Show).Select(x => new
            {
                name = EnumHelper<AllEnums.SendType>.GetDescription(Enum.GetName(typeof(AllEnums.SendType), x.Type)),
                value = x.Id
            }).OrderBy(x => x.name).ToList();

            ViewBag.productType = db.ProductTypes.Where(x => x.Status == (byte)AllEnums.Status.Show).Select(x => new
            {
                name = x.Title,
                value = x.Id
            }).OrderBy(x => x.name).ToList();

        }
        private DynamicFieldRepository _reopsitory = new DynamicFieldRepository(new DalEntities());

        // GET: admin/Product
        public ActionResult Index(int? page, string S_Title, string Status, int? Group, string S_Code, int? S_Brand, bool? S_EndingStock, bool? S_EndedStock, int? S_Sort)

        {

            if (Status != null)
                ViewBag.Status = Status;
            if (S_Title != null)
                ViewBag.S_Title = S_Title;
            if (Group != null)
                ViewBag.S_Group = Group;
            if (S_Brand != null)
                ViewBag.S_Brand = S_Brand;
            if (S_Code != null)
                ViewBag.S_Code = S_Code;
            if (S_EndingStock != null)
                ViewBag.S_EndingStock = S_EndingStock;
            if (S_EndedStock != null)
                ViewBag.S_EndedStock = S_EndedStock;
            if (S_Sort != null)
                ViewBag.S_Sort = S_Sort;



            this.ShowMessage();
            var list = db.Products.Where(p => p.Culture == "fa-IR" && p.IsDeleted == false).ToList();
            list = list.Where(p =>
            (S_Title == null || S_Title == "" || p.Title.Contains(S_Title)) &&
            (Status == null || Status == "" || p.Status == Convert.ToSByte(Status)) &&
            (S_Brand == null || p.BrandId == S_Brand) &&
            (S_EndingStock != true || p.ProductColors.Any(x => (x.Stock - x.WarningPoint) <= 0)) &&
            (S_EndedStock != true || p.ProductColors.Any(x => (x.Stock - x.WarningPoint) == 0)) &&
            (Group == 0 || Group == null || p.ProductGroupId == Group)
            ).OrderByDescending(p => p.DateX).ToList();

            switch (S_Sort)
            {
                case 1:
                    break;
                case 2:
                    list = list.OrderByDescending(p => p.ProductColors.Sum(x => x.SellCount)).ToList();
                    break;
                case 3:
                    list = list.OrderBy(p => p.ProductColors.Sum(x => x.SellCount)).ToList();
                    break;
                case 4:
                    list = list.OrderBy(p => p.DateX).ToList();
                    break;
                case 5:
                    list = list.OrderByDescending(p => p.DateX).ToList();
                    break;
                case 6:
                    list = list.OrderByDescending(p => p.Viewcount).ToList();
                    break;
                default:
                    break;
            }


            Iniital();
            int pageSize = ModulePageContent.GetPageSize();
            int pageNumber = (page ?? 1);
            return View(list.AsEnumerable().OrderBy(p => p.ShowOrder).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            Iniital();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product Product, FormCollection form)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DynamicFrm = _reopsitory.ShowDynamicFiledValues(form, Product);

                Iniital();
                return View(Product);
            }
            else
            {
                if (db.Products.Any(x => x.TitleURL == Product.TitleURL))
                {
                    ModelState.AddModelError("TitleURL", "آدرس تکراری می باشد");
                    Iniital();
                    return View(Product);
                }
                try
                {
                    Product.HtmlBody = HttpUtility.HtmlDecode(Product.HtmlBody);
                    Product.Feature = HttpUtility.HtmlDecode(Product.Feature);
                    db.Products.Add(Product);
                    db.SaveChanges(Request.Path);

                    _reopsitory.SaveDynamicFiledValues(form, Product);

                    Alert("عملیات با موفقیت انجام گردید");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.DynamicFrm = _reopsitory.ShowDynamicFiledValues(form, Product);
                    Iniital();
                    return View(Product);
                }
            }
        }


        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            Product Products = db.Products.Find(id);
            if (Products == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            ViewBag.DynamicFrm = _reopsitory.ShowDynamicFiledValues(null, Products);
            Iniital();
            return View(Products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product _Product, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                if (db.Products.Any(x => x.TitleURL == _Product.TitleURL && x.Id != _Product.Id))
                {
                    ModelState.AddModelError("TitleURL", "آدرس تکراری می باشد");
                    return View(_Product);
                }

                _Product.HtmlBody = HttpUtility.HtmlDecode(_Product.HtmlBody);
                _Product.Feature = HttpUtility.HtmlDecode(_Product.Feature);
                db.Entry(_Product).State = EntityState.Modified;
                db.SaveChanges(Request.Path);


                _reopsitory.SaveDynamicFiledValues(form, _Product);
                Alert("عملیات با موفقیت انجام گردید");
                return RedirectToAction("Index");
            }
            ViewBag.DynamicFrm = _reopsitory.ShowDynamicFiledValues(form, _Product);
            Iniital();
            return View(_Product);
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            Product Product = db.Products.Find(id);
            if (Product == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            else
            {
                if (Product.ProductAmazings.Any())
                {
                    foreach (var item in db.ProductAmazings.Where(x => x.ProductId == Product.Id))
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }
                if (Product.ProductColors.Any())
                {
                    foreach (var item in db.ProductColors.Where(x => x.ProductId == Product.Id))
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }
                if (Product.ProductGalleries.Any())
                {
                    foreach (var item in db.ProductGalleries.Where(x => x.ProductId == Product.Id))
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }
                if (Product.Comments.Any())
                {
                    foreach (var item in db.CommentRates.Where(x => x.ProductId == Product.Id))
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                    foreach (var item in db.Comments.Where(x => x.ProductId == Product.Id))
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }
                if (db.ProductFaqAnswers.Where(X => X.ProductId == Product.Id).Any())
                {
                    foreach (var item in db.ProductFaqAnswers.Where(X => X.ProductId == Product.Id))
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }


                }
                db.Entry(Product).State = EntityState.Deleted;
                db.SaveChanges(Request.Path);
                Alert("عملیات با موفقیت انجام گردید");
            }
            return RedirectToAction("Index");
        }


        #region Gallery
        public ActionResult gallery(int? id, int? page, string S_Title)
        {
            if (S_Title != null)
                ViewBag.S_Title = S_Title;

            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            Product _Product = db.Products.Find(id);
            if (_Product == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            if (TempData["shortMessage"] != null)
            {
                ViewBag.SuccessMsg = TempData["shortMessage"].ToString();
                TempData.Remove("shortMessage");
            }
            var list = db.ProductGalleries.Where(p => p.ProductId == id).ToList();
            list = list.Where(p => (S_Title == null || p.Title.Contains(S_Title))).ToList();

            int pageSize = ModulePageContent.GetPageSize();
            int pageNumber = (page ?? 1);
            ViewBag.ProductName = _Product.Title;
            ViewBag.Id = _Product.Id;
            return View(list.AsEnumerable().OrderBy(p => p.ShowOrder).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Creategallery(int id)
        {
            Product _Product = db.Products.Find(id);
            if (_Product == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            ProductGallery _ProductGallery = new ProductGallery(id, _Product.Title);
            return View(_ProductGallery);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Creategallery(ProductGallery _ProductGallery)
        {
            if (!ModelState.IsValid)
            {
                return View(_ProductGallery);
            }
            else
            {
                try
                {
                    if (_ProductGallery.AttachedFile != null)
                    {
                        _ProductGallery.TypeGallery = (byte)AllEnums.TypeProductGallery.Video;
                    }
                    else
                    {
                        _ProductGallery.TypeGallery = (byte)AllEnums.TypeProductGallery.image;

                    }
                    db.ProductGalleries.Add(_ProductGallery);
                    db.SaveChanges(Request.Path);
                    Alert("عملیات با موفقیت انجام گردید");
                    return RedirectToAction("gallery", new RouteValueDictionary(new { controller = "Product", action = "gallery", Id = _ProductGallery.ProductId }));

                }
                catch (Exception ex)
                {
                    return View(_ProductGallery);
                }
            }
        }


        public ActionResult Editgallery(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            ProductGallery _ProductGallery = db.ProductGalleries.Find(id);
            if (_ProductGallery == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View(_ProductGallery);
        }

        // POST: ProductGallery/Edit/5
        // To protect from overposting attacks,please enable the specific properties you want to bind to,for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editgallery(ProductGallery _ProductGallery)
        {
            if (ModelState.IsValid)
            {
                db.Entry(_ProductGallery).State = EntityState.Modified;
                db.SaveChanges(Request.Path);
                Alert("عملیات با موفقیت انجام گردید");
                return RedirectToAction("gallery", new RouteValueDictionary(new { controller = "Product", action = "gallery", Id = _ProductGallery.ProductId }));
            }

            return View(_ProductGallery);
        }
        public ActionResult Deletegallery(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            ProductGallery _ProductGallery = db.ProductGalleries.Find(id);
            int Pid = Convert.ToInt32(_ProductGallery.ProductId);
            if (_ProductGallery == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            else
            {

                db.Entry(_ProductGallery).State = EntityState.Deleted;
                db.SaveChanges(Request.Path);
                Alert("عملیات با موفقیت انجام گردید");
            }

            return RedirectToAction("gallery", new RouteValueDictionary(new { controller = "Product", action = "gallery", Id = Pid }));
        }
        #endregion


        #region GetColorList

        public ActionResult GetColorList(int ID)
        {
            var list = db.ProductColors.Where(x => x.ProductId == ID && x.IsDeleted == false).ToList();
            return PartialView("_price", list.AsEnumerable().OrderByDescending(p => p.DateX).ToList());
        }



        [HttpPost]
        public ActionResult SaveColorList(List<ProductColor> _ProductColors)
        {
            foreach (var item in _ProductColors)
            {
                var result = db.ProductColors.Find(item.Id);
                result.OtherCode = item.OtherCode;
                result.Stock = item.Stock;
                result.WarningPoint = item.WarningPoint;
                result.Price = item.Price;
                result.Discount = item.Discount;
            }
            db.SaveChanges();
            return PartialView("_sucsess");
        }
        #endregion

        #region Execl
        public ActionResult Execl(string S_Title, string Status, int? Group, string S_Code, int? S_Brand, bool? S_EndingStock, bool? S_EndedStock, int? S_Sort)
        {

            if (Status != null)
                ViewBag.Status = Status;
            if (S_Title != null)
                ViewBag.S_Title = S_Title;
            if (Group != null)
                ViewBag.S_Group = Group;
            if (S_Brand != null)
                ViewBag.S_Brand = S_Brand;
            if (S_Code != null)
                ViewBag.S_Code = S_Code;
            if (S_EndingStock != null)
                ViewBag.S_EndingStock = S_EndingStock;
            if (S_EndedStock != null)
                ViewBag.S_EndedStock = S_EndedStock;
            if (S_Sort != null)
                ViewBag.S_Sort = S_Sort;

            var list = db.Products.Where(p => p.Culture == "fa-IR").ToList();
            list = list.Where(p =>
            (S_Title == null || S_Title == "" || p.Title.Contains(S_Title)) &&
            (Status == null || Status == "" || p.Status == Convert.ToSByte(Status)) &&
            (S_Brand == null || p.BrandId == S_Brand) &&
            (S_EndingStock != true || p.ProductColors.Any(x => (x.Stock - x.WarningPoint) <= 0)) &&
            (S_EndedStock != true || p.ProductColors.Any(x => (x.Stock - x.WarningPoint) == 0)) &&
            (Group == 0 || Group == null || p.ProductGroupId == Group)
            ).OrderByDescending(p => p.DateX).ToList();

            switch (S_Sort)
            {
                case 1:
                    break;
                case 2:
                    list = list.OrderByDescending(p => p.ProductColors.Sum(x => x.SellCount)).ToList();
                    break;
                case 3:
                    list = list.OrderBy(p => p.ProductColors.Sum(x => x.SellCount)).ToList();
                    break;
                case 4:
                    list = list.OrderBy(p => p.DateX).ToList();
                    break;
                case 5:
                    list = list.OrderByDescending(p => p.DateX).ToList();
                    break;
                case 6:
                    list = list.OrderByDescending(p => p.Viewcount).ToList();
                    break;
                default:
                    break;
            }
            var P = (from product in list
                     select new
                     {
                         A1 = product.Title,
                         A2 = product.PageTitle,
                         A3 = product.TitleURL,
                         A4 = product.PageDCSubject,
                         A5 = product.PageDescription,
                         A6 = product.Feature,
                         A7 = product.Brand.Title,
                         A9 = product.ProductGroup.Title,
                     }).ToList();

            DataTable DT = new DataTable();
            DT.Columns.Add("عنوان", typeof(string));
            DT.Columns.Add("متاتگ عنوان", typeof(string));
            DT.Columns.Add("عنوان آدرس", typeof(string));
            DT.Columns.Add("متاتگ DC.Subject", typeof(string));
            DT.Columns.Add("متاتگ توضیحات", typeof(string));
            DT.Columns.Add("ویژگی", typeof(string));
            DT.Columns.Add("برند", typeof(string));
            DT.Columns.Add("گروه محصول", typeof(string));

            foreach (var item in P)
            {
                DataRow Row = DT.NewRow();
                Row["عنوان"] = item.A1;
                Row["متاتگ عنوان"] = item.A2;
                Row["عنوان آدرس"] = item.A3;
                Row["متاتگ DC.Subject"] = item.A4;
                Row["متاتگ توضیحات"] = item.A5;
                Row["ویژگی"] = item.A6;
                Row["برند"] = item.A7;
                Row["گروه محصول"] = item.A9;

                DT.Rows.Add(Row);
            }

            DataSet DS = new DataSet();
            DS.Tables.Add(DT);

            excel.ExportToExcel2(DS, "Product.xls");


            Iniital();
            int pageSize = ModulePageContent.GetPageSize();
            int pageNumber = (1);
            Alert("عملیات با موفقیت انجام گردید");
            return View("Index", list.AsEnumerable().OrderBy(p => p.ShowOrder).ToPagedList(pageNumber, pageSize));
        }

        #endregion


        #region Faq

        public ActionResult Faqs(int? id, int? page)
        {

            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            if (TempData["shortMessage"] != null)
            {
                ViewBag.SuccessMsg = TempData["shortMessage"].ToString();
                TempData.Remove("shortMessage");
            }
            var list = db.Faqs.Where(x => x.ProductId == id).ToList();
            if (list == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            ViewBag.Id = id;
            int pageSize = ModulePageContent.GetPageSize();
            int pageNumber = (page ?? 1);
            return View(list.AsEnumerable().OrderBy(p => p.ShowOrder).ToPagedList(pageNumber, pageSize));

        }

        public ActionResult CreateFaq(int? id)
        {
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFaq(Faq Faq, int? id)
        {
            if (!ModelState.IsValid)
            {

                return View(Faq);
            }
            else
            {
                try
                {
                    Faq.Name = "admin";
                    Faq.Email = "";
                    db.Faqs.Add(Faq);
                    db.Entry(Faq).State = EntityState.Added;
                    db.SaveChanges(Request.Path);
                    Alert("عملیات با موفقیت انجام گردید");
                    return RedirectToAction("Faqs", new { id = id });
                }
                catch (Exception ex)
                {
                    return View(Faq);
                }
            }
        }


        // GET: StateCities/Edit/5
        public ActionResult EditFaq(int? id)
        {
            if (id == null)
            {
                return Redirect("/404");
            }
            Faq Faq = db.Faqs.Find(id);
            if (Faq == null)
            {
                return Redirect("/404");
            }

            return View(Faq);
        }

        // POST: Faq/Edit/5
        // To protect from overposting attacks,please enable the specific properties you want to bind to,for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFaq(Faq Faq)
        {
            if (ModelState.IsValid)
            {
                Faq.Writer = 0;
                db.Entry(Faq).State = EntityState.Modified;
                db.SaveChanges(Request.Path);
                Alert("عملیات با موفقیت انجام گردید");
                return RedirectToAction("Faqs", new { id = Faq.ProductId });
            }
            return View(Faq);
        }



        public ActionResult DeleteFaq(int? id)
        {
            if (id == null)
            {
                return Redirect("/404");
            }
            Faq Faq = db.Faqs.Find(id);
            var producutId = Faq.ProductId;
            if (Faq == null)
            {
                return Redirect("/404");
            }
            else
            {
                db.Entry(Faq).State = EntityState.Deleted;
                db.SaveChanges(Request.Path);
                Alert("عملیات با موفقیت انجام گردید");
            }
            return RedirectToAction("Faqs",new { id = producutId });
        }


        #endregion

    }
}