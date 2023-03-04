using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kaspid.Models;
using Kaspid.Models.Repository;
using Kaspid.Models.Utility;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace Kaspid.Controllers
{
    public class ProductController : InsideController
    {
        #region Fields

        DalEntities db = new DalEntities();
        DateTime dt = DateTime.Now;

        #region Init


        private void Init(int id)
        {
            ViewBag.CmsPage = db.ProductGroups.FirstOrDefault(x => x.Id == id) ?? new ProductGroup();

        }

        #endregion

        #endregion

        #region Index
        [OutputCache(CacheProfile = "Page")]
        [WebMarkupMin.AspNet4.Mvc.MinifyHtml]

        public ActionResult Index()
        {
            #region Cms

            ViewBag.CmsPage = db.CmsPages.FirstOrDefault(x => x.Url.ToLower().Trim() == "products") ?? new CmsPage();

            #endregion

            ViewBag.productGroup = db.ProductGroups.AsEnumerable().Where(x => x.Status == Status && x.IsDeleted == false && x.ParentId == null && x.Id != 5)
                 .OrderBy(x => x.ShowOrder).AsEnumerable()
                .Select(p => new
                {
                    p.Id,
                    p.TitleURL,
                    p.Title,
                    p.Picture
                });
            return View();
        }

        #endregion

        #region Lst


        [OutputCache(CacheProfile = "Page", VaryByParam = "title;query;page")]
        [WebMarkupMin.AspNet4.Mvc.MinifyHtml]
        public ActionResult List(string title, string query, int? page)
        {

            var pgroup = db.ProductGroups.FirstOrDefault(p => p.TitleURL == title);
            if (pgroup == null || pgroup.IsDeleted == true || pgroup.Status == (byte)AllEnums.Status.hide)
                return View("PageNotFound");

            List<int> lst = db.getAllChildIds(pgroup.Id);
            Init(pgroup.Id);
            if (query != null)
            {
                Filtering result = JsonConvert.DeserializeObject<Filtering>(query);
            }

            #region Brand
            var brand = db.Products.Where(x => x.Status == (byte)AllEnums.Status.Show && lst.Contains(x.ProductGroupId) && x.IsDeleted == false && x.BrandId != null)
              .GroupBy(x => new { x.Brand.Id, x.Brand.Title })
              .Select(x => new { x.Key.Id, x.Key.Title, Count = x.Count() })
              .ToList();
            TempData["Brand"] = brand;
            #endregion

            #region Color
            var color = db.ProductColors.Where(x => x.Status == (byte)AllEnums.Status.Show && lst.Contains(x.Product.ProductGroupId) && x.Product.Status == (byte)AllEnums.Status.Show)
              .GroupBy(x => new { x.Color.Id, x.Color.Title, x.Color.Code })
              .Select(x => new { x.Key.Id, x.Key.Title, x.Key.Code })
              .ToList();
            TempData["Color"] = color;
            #endregion

            #region Type
            var type = db.ProductColors.Where(x => x.Status == (byte)AllEnums.Status.Show && lst.Contains(x.Product.ProductGroupId) && x.Product.Status == (byte)AllEnums.Status.Show)
               .GroupBy(x => new { Id = x.ProductType.Id != null ? x.ProductType.Id : 0, Title = x.ProductType.Title != null ? x.ProductType.Title : "" })
                .Select(x => new { x.Key.Id, x.Key.Title })
               .ToList();
            if (type != null)
            {
                TempData["Type"] = type;
            }
            #endregion

            #region ProductGroup

            var groups = db.ProductGroups.AsEnumerable().Where(x => x.Status == (byte)AllEnums.Status.Show && x.ParentId == pgroup.ParentId && x.IsDeleted == false)
           .Select(x => new
           {
               x.Id,
               x.Title,
               x.TitleURL,
               CountProduct = x.Products.Where(P => P.Status == Status && P.IsDeleted == false && (P.ProductGroupId == x.Id || db.getAllChildIds(x.Id).Contains(P.ProductGroupId))).Count(),
           })
           .ToList();
            ViewData["Group"] = groups;
            #endregion

            #region Related Product

            var related = db.Products.Where(x => x.IsDeleted == false && x.Status == Status && x.ProductGroupId != pgroup.Id && x.ProductGroup.ParentId == pgroup.ParentId).OrderByDescending(x => x.Viewcount).AsEnumerable().Select(p => new
            {
                p.Id,
                p.Title,
                p.TitleURL,
                p.Picture,
                p.DateX,
                SellCount = p.SumSellcount,
                p.Price,
                p.FinalPrice,
                p.Discount,
                p.Viewcount,
               // Rate = p.ProductRates.Count() > 0 ? p.ProductRates.Average(x => x.Rate) : 0,
                colors = p.ProductColors.Where(x => x.ProductId == p.Id).ToList().Select(x => new { Code = x.Color.Code }).ToList(),
                ProductColors = p.ProductColors.Count > 0 ? p.ProductColors.FirstOrDefault().Price : 0,
                //amazing = p.ProductAmazings.Count() > 0 ? p.ProductAmazings.Where(x => x.Product.IsDeleted == false && x.StartDate <= dt && x.EndDate >= dt && x.Status == 2).FirstOrDefault() : null,
              //  edate = p.ProductAmazings.Count() > 0 ? p.ProductAmazings.Where(x => x.Product.IsDeleted == false && x.StartDate <= dt && x.EndDate >= dt && x.Status == 2).FirstOrDefault().EndDate : p.DateX,
               // isfav = Convert.ToInt32(SiteUtility.UserIsLogin || SiteUtility.UserIsSiteUser ? db.Favorites.Where(q => q.ProductId == p.Id && q.UserId == SiteUtility.UserId).Count() : 0),
                IsStock = p.IsStock
            })
             .ToList();

            #endregion

            #region Offer Product

            ViewData["Offers"] = db.Products.Where(x => x.IsDeleted == false && x.Status == 2 && x.showinfilter == true && x.ProductGroup.BrandId == pgroup.BrandId).AsEnumerable().Select(p => new
            {
                p.Id,
                p.Title,
                TitleURL = p.TitleURL.ToLower().Trim(),
                p.Picture,
                p.DateX,
                SellCount = p.SumSellcount,
                p.Price,
                p.FinalPrice,
                p.Discount,
                p.Viewcount,
                Rate = p.ProductRates.Count() > 0 ? p.ProductRates.Average(x => x.Rate) : 0,
                colors = p.ProductColors.Where(x => x.ProductId == p.Id).ToList().Select(x => new { Code = x.Color.Code }).ToList(),
                ProductColors = p.ProductColors.Count > 0 ? p.ProductColors.FirstOrDefault().Price : 0,
                amazing = p.ProductAmazings.Count() > 0 ? p.ProductAmazings.Where(x => x.Product.IsDeleted == false && x.StartDate <= dt && x.EndDate >= dt && x.Status == 2).FirstOrDefault() : null,
                edate = p.ProductAmazings.Count() > 0 ? p.ProductAmazings.Where(x => x.Product.IsDeleted == false && x.StartDate <= dt && x.EndDate >= dt && x.Status == 2).FirstOrDefault().EndDate : p.DateX,
                isfav = Convert.ToInt32(SiteUtility.UserIsLogin || SiteUtility.UserIsSiteUser ? db.Favorites.Where(q => q.ProductId == p.Id && q.UserId == SiteUtility.UserId).Count() : 0),
                IsStock = p.IsStock
            })
             .ToList();

            #endregion
            if (pgroup.Brand != null)
            {
                ViewBag.OfferTitle = pgroup.Brand.Title;
                ViewBag.OfferPic = pgroup.Brand.Picture;
            }

            #region Banner

            ViewData["InnerBanner"] = db.Banners
               .Where(p => p.Culture == "fa-IR" && p.Position == (byte)AllEnums.BannerPosition.ProductGroup && p.AddressPage != null && p.Status == this.Status && p.StartDate <= dt && p.EndDate >= dt)
               .OrderBy(p => p.ShowOrder).ToCacheableList();

            #endregion

            ViewBag.ProductgroupId = pgroup.Id;
            ViewBag.BreadcrumbList = GetBreadCrump(pgroup);
            ViewBag.SpecialProduct = db.Products.Where(x => x.ProductGroupId == pgroup.Id && x.Status == 2 && x.IsDeleted == false).FirstOrDefault(x => x.specialGroup == true);

            return View(related);
        }

        #endregion

        #region QuickView
        [HttpPost]

        public ActionResult QuickView(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return Redirect("/404");
            if (product.Status != Status || product.ProductGroup.Status != Status || product.ProductGroup.IsDeleted == false || product.IsDeleted == false)
                ViewBag.FaveCount = db.Favorites.Where(p => p.ProductId == id && p.UserId == SiteUtility.UserId).Count();
            ViewBag.Rate = product.ProductRates.Count() > 0 ? product.ProductRates.Average(x => x.Rate) : 0;
            return PartialView("_QuickView", product);
        }

        #endregion

        #region AlertStock
        [HttpPost]
        [ValidateAntiForgeryToken]

        public JsonResult AlertStock(AlertStock _AlertStock)
        {
            if (Session["AlertStock" + _AlertStock.ProductColorId] == null)
            {
                if (SiteUtility.UserIsLogin && SiteUtility.UserIsSiteUser)
                {
                    _AlertStock.Name = SiteUtility.UserInfo.FirstName + " " + SiteUtility.UserInfo.LastName;
                    _AlertStock.Email = SiteUtility.UserInfo.Email;
                    _AlertStock.Mobile = SiteUtility.UserInfo.UserName;
                    _AlertStock.UserId = SiteUtility.UserId;


                    db.AlertStocks.Add(_AlertStock);
                    db.SaveChanges();
                    Session["AlertStock" + _AlertStock.ProductColorId] = "true";
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region DynamicFiled

        public void GetFields(int ID, int? productId)
        {
            DynamicFieldRepository _repository = new DynamicFieldRepository(db);
            Product product = new Product();
            product.ProductGroupId = ID;
            product.Id = productId.HasValue ? Convert.ToInt32(productId) : 0;
            var lst = _repository.ShowDynamicFiledValues(null, product);
            ViewBag.DynamicFields = lst;
        }

        #endregion

        #region AddFav

        [HttpPost]

        public JsonResult AddFav(int id)
        {
            var userId = User.Identity.GetUserId();

            var add = new Favorites()
            {
                ProductId = id,
                UserId = userId
            };
            if (!(db.Favorites.Any(p => p.UserId == userId && p.ProductId == id)))
            {
                db.Favorites.Add(add);
                db.SaveChanges();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region SendEmail

        [HttpPost]

        public JsonResult SendEmail(string email, string link)
        {
            var body =
                "کاربر عزیز  " + "<br/>"
                + "پست زیر را دنبال کن" +
                "<br/><br/>" +
                link;
            SiteUtility.SendMail(email, "اشتراک گذاری پست آتی میکاپ ", body);
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Price

        public string Price(int id)
        {
            string RetVal = "";
            var price = db.ProductColors.Find(id);
            var pro = db.Products.Find(price.ProductId);
            if (price != null)
            {
                if (price.HasStock)
                {
                    if (price.Price != price.FinalPrice)
                    {
                        RetVal = "<del>" + Function.makeprice(price.Price) + "</del>";
                    }
                    RetVal +=
            "<p>" + Function.makeprice(price.FinalPrice) + "ریال</p>" +
                                "<a  href='/basket/add/" + price.Id + "/1/' class='basket-box add__basket_detail'>افزودن به سبد خرید</a>";

                    return "-" + RetVal;
                }
                else
                {
                    if (price.Price != price.FinalPrice)
                    {
                        RetVal = "<del>" + Function.makeprice(price.Price) + "</del>";
                    }
                    RetVal +=
            "<p>" + Function.makeprice(price.FinalPrice) + "ریال</p>" +
                                "<a  href='/Order/" + pro.Id + "/" + pro.SelectedProductColor.Id + "' class='basket-box add__basket_detail'>افزودن به سبد خرید</a>";

                    return "-" + RetVal;
                }
            }
            return "n";
        }

        #endregion

        #region Faq

        public ActionResult Faq(Faq faq)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Faq", faq);
            }
            else
            {

                faq.DateX = DateTime.Now;
                db.Faqs.Add(faq);
                db.Entry(faq).State = EntityState.Added;
                db.SaveChanges();
                ModelState.Clear();
                return PartialView("_successFaq");
            }
        }

        #endregion

        #region Detail
        [OutputCache(CacheProfile = "Page", VaryByParam = "title")]
        [WebMarkupMin.AspNet4.Mvc.MinifyHtml]

        public ActionResult Detail(int? id, string title)
        {
            if (id == null)
                return Redirect("/404");
            var product = db.Products.Find(id);
            if (product == null)
                return Redirect("/404");

            if (product.Status != Status || product.ProductGroup.Status != Status || product.ProductGroup.IsDeleted || product.IsDeleted)
                return Redirect("/404");
            ViewBag.ProductID = product.Id;
            if (SiteUtility.UserIsLogin && SiteUtility.UserIsSiteUser)
            {
                var userId = SiteUtility.UserId;
                var user = db.Users.Find(userId);
                if (user.Favorites.Any(x => x.Product.TitleURL.ToLower().Trim() == title))
                {
                    ViewBag.ActiveFavorite = true;
                }
                ViewBag.FullName = user.FirstName + " " + user.LastName;
                ViewBag.Email = user.Email;
            }

            ViewBag.ProductColor = db.ProductColors.Where(x => x.Status == Status && !x.IsDeleted && x.ColorId != 1 &&  x.ProductId==product.Id).Select(p => new { Id = p.Id, ColorCode = p.Color.Code }).ToList();
            #region viewcount

            if (Session["ViewCountProduct_" + product.Id] == null)
            {
                product.Viewcount++;
                db.SaveChanges();
                Session["ViewCountProduct_" + product.Id] = "true";
            }

            #endregion
            #region GetRelatedProducts

            GetRelatedProducts(product.ProductGroupId, product.Id);

            #endregion

            #region Faqpro

            if (SiteUtility.UserIsLogin && SiteUtility.UserIsSiteUser)
            {
                var userId = SiteUtility.UserId;
                var user = db.Users.Find(userId);
                if (user.Favorites.Any(x => x.Product.TitleURL.ToLower().Trim() == title))
                {
                    ViewBag.ActiveFavorite = true;
                }
                ViewBag.FullName = user.FirstName + " " + user.LastName;
                ViewBag.Email = user.Email;
            }

            GetFields(product.ProductGroupId, product.Id);

            ViewData["productFeatureFaq"] = db.ProductFeatureFaqs.Where(x => x.Status == Status && x.IsDeleted == false && x.ProductGroupId == product.ProductGroupId).OrderBy(x => x.ShowOrder).ToList();
            ViewBag.One = db.CommentRates.Where(x => x.Product.TitleURL.ToLower().Trim() == title).Count(x => x.Rate >= 1 && x.Rate <= 2);
            ViewBag.Two = db.CommentRates.Where(x => x.Product.TitleURL.ToLower().Trim() == title).Count(x => x.Rate >= 2 && x.Rate <= 3);
            ViewBag.Three = db.CommentRates.Where(x => x.Product.TitleURL.ToLower().Trim() == title).Count(x => x.Rate >= 3 && x.Rate <= 4);
            ViewBag.Four = db.CommentRates.Where(x => x.Product.TitleURL.ToLower().Trim() == title).Count(x => x.Rate >= 4 && x.Rate <= 5);
            ViewBag.Five = db.CommentRates.Where(x => x.Product.TitleURL.ToLower().Trim() == title).Count(x => x.Rate == 5);

            #endregion

            #region GetPriceChart

           // ViewBag.PriceChart = 
                GetPriceChart(product.Id);

            #endregion

            #region Comment

            ViewBag.Comments = db.Comments.AsEnumerable()
            .Where(x => x.Status == Status && x.IsDeleted == false && x.ProductId == product.Id)
            .Select(p => new
            {
                p.Id,
                Name = p.Name,
                DateX = p.DateX,
                Rate = p.CommentRates.Count() > 0 ? p.CommentRates.FirstOrDefault().Rate : 0,
                LikeCount = p.LikeCount,
                DisLikeCount = p.DisLikeCount,
                Title = p.Title,
                TextX = p.TextX,
                OrderStatus = p.OrderStatus,
                p.Guide,
                UserOrderDetailId = p.UserOrderDetailId != null ? true : false,
                colorCode = (p.UserOrderDetailId != null ? db.UserOrderDetails.Find(p.UserOrderDetailId).ProductColor.Color.Code : ""),
                color = (p.UserOrderDetailId != null ? db.UserOrderDetails.Find(p.UserOrderDetailId).ProductColor.Color.Title : ""),
                FinalPrice = p.UserOrderDetailId != null ? db.UserOrderDetails.Find(p.UserOrderDetailId).Price : 0

            }).ToList();

            ViewBag.totalVote = product.ProductRates.Count() > 0 ? product.ProductRates.Average(x => x.Rate) : 0;

            #endregion

            #region GetRateProduct

            GetRateProduct(product.Id);

            #endregion
            if ((db.ProductGroups.Find(product.ProductGroupId).ParentId) == 5)
            {
                ViewBag.BreadCrump = GetBreadCrumpBook(product);

            }
            else
            {
                ViewBag.BreadCrump = GetBreadCrump(product);
            }

            #region Query

            ViewData["RlProduct"] = db.Products.Where(x => x.ProductGroupId == product.ProductGroupId && x.Id != product.Id && x.IsDeleted == false && x.Status == 2).OrderBy(x => x.ShowOrder).AsEnumerable().Select(p => new
            {
                //p.Id,
                //p.Title,
                //TitleURL = p.TitleURL.ToLower().Trim(),
                //p.Picture,
                //p.DateX,
                //SellCount = p.SumSellcount,
                //p.Price,
                //p.FinalPrice,
                //p.Discount,
                //p.Viewcount,
                //Rate = p.ProductRates.Count() > 0 ? p.ProductRates.Average(x => x.Rate) : 0,
                //colors = p.ProductColors.Where(x => x.ProductId == p.Id).ToList().Select(x => new { Code = x.Color.Code }).ToList(),
                //ProductColors = p.ProductColors.Count > 0 ? p.ProductColors.FirstOrDefault().Price : 0,
                //amazing = p.ProductAmazings.Count() > 0 ? p.ProductAmazings.Where(x => x.Product.IsDeleted == false && x.StartDate <= dt && x.EndDate >= dt && x.Status == 2).FirstOrDefault() : null,
                //edate = p.ProductAmazings.Count() > 0 ? p.ProductAmazings.Where(x => x.Product.IsDeleted == false && x.StartDate <= dt && x.EndDate >= dt && x.Status == 2).FirstOrDefault().EndDate : p.DateX,
                //isfav = Convert.ToInt32(SiteUtility.UserIsLogin || SiteUtility.UserIsSiteUser ? db.Favorites.Where(q => q.ProductId == p.Id && q.UserId == SiteUtility.UserId).Count() : 0)


            })
             .ToList();

            ViewData["ProductFaq"] = db.Faqs.Where(x => x.Writer == 0 && x.ProductId == product.Id).OrderBy(x => x.ShowOrder).ToList();

            #region محصولات با هم خریداری شده

            List<int> Ids = db.UserOrderDetails.Where(p => p.ProductColor.ProductId == product.Id && p.UserOrder.IsDeleted == false).Select(p => p.UserOrderId).ToList();
            List<int> PIds = db.UserOrderDetails.Where(p => p.UserOrder.IsDeleted == false && Ids.Contains(p.UserOrderId)).Select(p => p.ProductColor.ProductId).Distinct().ToList();

            ViewData["Buying"] = db.Products.AsEnumerable()
               .Where(p => PIds.Contains(p.Id) && p.Id != product.Id && p.IsDeleted == false && p.Status == 2 && p.ProductGroup.IsDeleted == false && p.ProductGroup.Status == 2)
              .Select(x =>
            new
            {
                x.Id,
                x.Title,
                TitleURL = x.TitleURL.ToLower().Trim(),
                x.Picture,
                x.DateX,
                SellCount = x.SumSellcount,
                x.Price,
                x.FinalPrice,
                x.Discount,
                x.Viewcount,
                Rate = x.ProductRates.Count() > 0 ? x.ProductRates.Average(z => z.Rate) : 0,
                colors = x.ProductColors.Where(z => z.ProductId == x.Id).ToList().Select(z => new { Code = z.Color.Code }).ToList(),
                ProductColors = x.ProductColors.Count > 0 ? x.ProductColors.FirstOrDefault().Price : 0,
                amazing = x.ProductAmazings.Count() > 0 ? x.ProductAmazings.Where(z => z.Product.IsDeleted == false && z.StartDate <= dt && z.EndDate >= dt && z.Status == 2).FirstOrDefault() : null,
                edate = x.ProductAmazings.Count() > 0 ? x.ProductAmazings.Where(z => z.Product.IsDeleted == false && z.StartDate <= dt && z.EndDate >= dt && z.Status == 2).FirstOrDefault().EndDate : x.DateX,
                isfav = Convert.ToInt32(SiteUtility.UserIsLogin || SiteUtility.UserIsSiteUser ? db.Favorites.Where(q => q.ProductId == x.Id && q.UserId == SiteUtility.UserId).Count() : 0)

            }).Take(8).ToList();
            #endregion

            #endregion


            return View(product);
        }
        #endregion

        #region GetRelatedProducts

        private void GetRelatedProducts(int productGroupId, int productId)
        {
            TempData["relatedProducts"] = db.Products
                .AsEnumerable()
                .Where(x => x.Status == Status && x.IsDeleted == false
                                               && x.ProductGroupId == productGroupId
                                               && x.Id != productId
                                               && x.SelectedProductColor != null
                                               )//&& !x.IsUnavailable
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.TitleURL,
                    x.Picture,
                    x.FinalPrice,
                    x.Discount,
                    x.Price,
                    Brand = x.Brand.Title,
                    //  x.IsUnavailable,
                    // x.IsBuyInstallment,
                    x.ShowInNewst,
                    x.SelectedProductColor.HasStock
                }).OrderByDescending(x => x.Id).Take(10).ToList();
        }

        #endregion

        #region BreadCrump

        protected string GetBreadCrumpBook(Product product)
        {
            String[] aray = new String[10];
            string ltrBreadCrump = "";
            int? parentid = product.ProductGroupId;
            var parentid1 = product.ProductGroup.ParentId;
            int i = 0, j = 2, k = 3;
            while (parentid1 != null && parentid1 != 0)
            {
                var queryItems = db.ProductGroups.FirstOrDefault(p => p.Id == parentid1 && p.IsDeleted == false);
                parentid1 = queryItems.ParentId;
                j++;
                k++;
            }
            while (parentid != null)
            {
                var queryItems = db.ProductGroups.FirstOrDefault(p => p.Id == parentid && p.IsDeleted == false);
                parentid = queryItems.ParentId;
                if (queryItems.Id != 5)
                {
                    aray[i] = string.Format("<li itemprop='itemListElement' itemscope itemtype='http://schema.org/ListItem'>" +
                        "<a itemprop='item' href='/Book/{0}' >{1}</a><meta itemprop='position' content='" + j + "' />" +
                        "<meta itemprop='name' content={1} /></li>", queryItems.TitleURL, queryItems.Title.SecureText());
                }
                j--;
                i++;
            }
            i = i - 1;
            while (i >= 0)
            {
                ltrBreadCrump += aray[i] + "  "; //»
                i--;
            }
            ltrBreadCrump = string.Format("<li itemprop='itemListElement' itemscope itemtype='http://schema.org/ListItem'><a itemprop='item'>کتاب ها </a><meta itemprop='position' content='" + j + "' /></li>") + ltrBreadCrump;
            ltrBreadCrump += string.Format("<li itemprop='itemListElement' itemscope itemtype='http://schema.org/ListItem'><a class='cur' href='/Book/detail/{0}/{1}' itemprop='item'>{2}</a> <meta itemprop='position' content='" + k + "' /><meta itemprop='name' content={2} /></li>", product.Id, product.TitleURL, product.Title.SecureText());
            return ltrBreadCrump;
        }

        protected string GetBreadCrump(ProductGroup productGroup)
        {
            String[] aray = new String[10];
            string ltrBreadCrump = "";
            var parentid = productGroup.ParentId;
            var parentid1 = productGroup.ParentId;
            int i = 1, j = 2, k = 3;

            aray[0] = string.Format("" +
                    "<li itemprop='itemListElement' itemscope itemtype='http://schema.org/ListItem'>" +
                    "<a itemprop='item' href='/products/list/{0}/' >{1}</a>" +
                    "<meta itemprop='position' content='" + j + "' />" +
                    "<meta itemprop='name' content={1} />" +
                    "</li>",
                    productGroup.TitleURL, productGroup.Title.SecureText());
            while (parentid1 != null && parentid1 != 0 && parentid1 != 1)
            {
                var queryItems = db.ProductGroups.FirstOrDefault(p => p.Id == parentid1 && p.IsDeleted == false);
                parentid1 = queryItems.ParentId;
                j++;
                k++;
            }
            while (parentid != null && parentid1 != 0 && parentid1 != 1)
            {
                var queryItems = db.ProductGroups.FirstOrDefault(p => p.Id == parentid && p.IsDeleted == false);
                parentid = queryItems.ParentId;
                aray[i] = string.Format("" +
                    "<li itemprop='itemListElement' itemscope itemtype='http://schema.org/ListItem'>" +
                    "<a itemprop='item' href='/products/list/{0}/' >{1}</a>" +
                    "<meta itemprop='position' content='" + j + "' />" +
                                        "<meta itemprop='name' content={1} />" +
                    "</li>",
                    Function.InsTitleUrl(queryItems.Title, queryItems.TitleURL), queryItems.Title.SecureText());
                j--;
                i++;
            }
            i = i - 1;
            while (i >= 0 && parentid1 != 0)
            {
                ltrBreadCrump += aray[i] + "  "; //»
                i--;
            }
            //ltrBreadCrump = string.Format("<li itemprop='itemListElement' itemscope itemtype='http://schema.org/ListItem'>" +
            //    "<a href='/products/' itemprop='item'>محصولات</a>" +
            //    "<meta itemprop='position' content='" + j + "' /></li>") + ltrBreadCrump;
            return ltrBreadCrump;
        }

        protected string GetBreadCrump(Product product)
        {
            String[] aray = new String[10];
            string ltrBreadCrump = "";
            int? parentid = product.ProductGroupId;
            var parentid1 = product.ProductGroup.ParentId;
            int i = 0, j = 2, k = 3;
            while (parentid1 != null && parentid1 != 0)//&& parentid1 != 1
            {
                var queryItems = db.ProductGroups.FirstOrDefault(p => p.Id == parentid1 && p.IsDeleted == false);
                parentid1 = queryItems.ParentId;
                j++;
                k++;
            }
            while (parentid != null)//&& parentid1 != 1
            {
                var queryItems = db.ProductGroups.FirstOrDefault(p => p.Id == parentid && p.IsDeleted == false);
                parentid = queryItems.ParentId;
                if (queryItems.Id != 1)
                    aray[i] = string.Format("<li itemprop='itemListElement' itemscope itemtype='http://schema.org/ListItem'>" +
                        "<a itemprop='item' href='/product/list/{0}/' >{1}</a>" +
                        "<meta itemprop='position' content='" + j + "' />" +
                        "<meta itemprop='name' content={1} /></li>", Function.InsTitleUrl(queryItems.Title, queryItems.TitleURL), queryItems.Title.SecureText());
                j--;
                i++;
            }
            i = i - 1;
            while (i >= 0)
            {
                ltrBreadCrump += aray[i] + "  "; //»
                i--;
            }
            //ltrBreadCrump = string.Format("<li itemprop='itemListElement' itemscope itemtype='http://schema.org/ListItem'>" +
            //    "<a href='/products/' itemprop='item'>محصولات</a>" +
            //    "<meta itemprop='position' content='" + j + "' /></li>") + ltrBreadCrump;
            ltrBreadCrump += string.Format("<li itemprop='itemListElement' itemscope itemtype='http://schema.org/ListItem'>" +
                "<a class='cur' href='/product/detail/{0}/{1}/' itemprop='item'>{2}</a>" +
                " <meta itemprop='position' content='" + k + "' />" +
                "<meta itemprop='name' content={2} /></li>", product.Id, Function.InsTitleUrl(product.Title, product.TitleURL), product.Title.SecureText());
            return ltrBreadCrump;
        }



        #endregion

        #region SendComment

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidationCaptcha]


        public ActionResult SendComment(Comment comment, int? id, FormCollection form, string captcha)
        {
            if (!ModelState.IsValid)
                return PartialView("_ProductComment", comment);
            var product = db.Products.Find(id);
            var list = db.ProductFeatureFaqs.Where(x => x.Status == Status && x.ProductGroupId == product.ProductGroup.Id || x.ProductGroupId == product.ProductGroup.ParentId).OrderBy(x => x.ShowOrder).ToList();
            comment.ProductId = id;
            comment.UserId = SiteUtility.UserId;
            comment.DateX = DateTime.Now;
            db.Comments.Add(comment);
            db.SaveChanges();
            if (list.Count != 0)
            {
                foreach (var item in list)
                {
                    var answer = db.ProductFaqAnswers.Create();
                    answer.ProductId = id;
                    answer.CommentId = comment.Id;
                    answer.ProductFeatureFaqId = item.Id;
                    var result = form["answer_" + item.Id];
                    if (result != null)
                    {
                        answer.Value = Convert.ToInt32(result);
                    }
                    db.ProductFaqAnswers.Add(answer);
                    db.SaveChanges();
                }
            }

            //var res = form["commentRate"].ToString();
            //if (res != "" || res != null)
            //{
            //    var start = new CommentRate()
            //    {
            //        ProductId = (int)id,
            //        CommentId = comment.Id,
            //        Rate = Convert.ToDecimal(res)
            //    };
            //    db.CommentRates.Add(start);
            //    db.SaveChanges();
            //    if (start != null)
            //    {
            //        return PartialView("_CommentResponse");
            //    }
            //}

            return PartialView("_success");
        }

        #endregion

        #region RateProduct

        [HttpPost]
        public JsonResult RateProduct(int? data, float rate)
        {
            //var rateStar = new ProductRate()
            //{
            //    ProductId = data,
            //};
            //if (Session["RateStarProduct_" + data] == null)
            //{
            //    rateStar.Rate = rate;
            //    db.ProductRates.Add(rateStar);
            //    db.SaveChanges();

            //    Session["RateStarProduct_" + data] = "true";
            //    return Json("success", JsonRequestBehavior.AllowGet);
            //}

            return Json("error", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region DisLike

        [HttpPost]

        public int DisLike(int id)
        {
            var comment = db.Comments.Find(id);
            if (Session["DislikeCommentCount_" + id] == null)
            {
                comment.DisLikeCount++;
                if (Session["likeCommentCount_" + id] != null)
                {
                    comment.LikeCount--;
                }
                db.SaveChanges();
                Session["DislikeCommentCount_" + id] = "true";
            }


            return comment.DisLikeCount;
        }

        #endregion

        #region LikeCount

        [HttpPost]
        public int LikeCount(int? id)
        {
            var comment = db.Comments.Find(id);
            if (Session["likeCommentCount_" + id] == null)
            {
                comment.LikeCount++;
                if (Session["DislikeCommentCount_" + id] != null)
                {
                    comment.DisLikeCount--;
                }
                db.SaveChanges();
                Session["likeCommentCount_" + id] = "true";
            }

            return comment.LikeCount;
        }

        [HttpPost]
        public int LikeCountPro(int? id)
        {
            var Product = db.Products.Find(id);
            if (Session["likePro_" + id] == null)
            {
                Product.LikeCount++;
                if (Session["DislikePro_" + id] != null)
                {
                    Product.DisLikeCount--;
                }
                db.SaveChanges();
                Session["likePro_" + id] = "true";
            }

            return Convert.ToInt32(Product.LikeCount);
        }

        #endregion

        #region AddToFavorite

        [HttpPost]
        public JsonResult AddToFavorite(int id)
        {
            var checkFavorites = db.Favorites
                .FirstOrDefault(x => x.ProductId == id && x.UserId == SiteUtility.UserId);
            if (checkFavorites != null)
                return Json("available", JsonRequestBehavior.AllowGet);

            var favorite = new Favorites()
            {
                UserId = SiteUtility.UserId,
                CreateDate = DateTime.Now,
                ProductId = id
            };

            db.Favorites.Add(favorite);
            db.SaveChanges();

            return Json("success", JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region GetPriceChart

        public string GetPriceChart(int productId)
        {
            string retVal = "";
            dt = dt.AddYears(-1);
            var product = db.Products.Find(productId);
            var ProductPricesMaterial = db.PriceLists
                .AsEnumerable()
                .Where(p => p.DateX >= dt.Date && p.ProductColorId == product.SelectedProductColor.Id)
                .Select(p => new
                {
                    Price = p.Price,
                    Date = p.DateX,
                }).OrderBy(p => p.Date).ToList();
            if (product.SelectedProductColor != null)
                retVal += "[" + (Convert.ToDateTime(DateTime.Now.Date).AddYears(-1969).Ticks / 10000) + "," + product.SelectedProductColor.Price + "],";
            string PriceCHart = "";
            string DateChart = "";
            string DesChart = "";
            PersianCalendar persianCalendar = new PersianCalendar();
            foreach (var item in ProductPricesMaterial)
            {
                retVal += "[" + (Convert.ToDateTime(item.Date).AddYears(-1969).Ticks / 10000) + "," + item.Price + "],";
                PriceCHart += item.Price + ",";
                DateChart += "'"+ Extensions.PersianMonthName((byte)persianCalendar.GetMonth(item.Date)) + " . " + persianCalendar.GetDayOfMonth(item.Date).ToString() + " ',";//"'"+Convert.ToDateTime(item.Date).ToPersianWithNamedMonth3()+"',";
                DesChart += "<div class='red'><b>"+Extensions.PersianDayOfWeek(item.Date)+ " "+ persianCalendar.GetDayOfMonth(item.Date).ToString() + " "+ Extensions.PersianMonthName((byte)persianCalendar.GetMonth(item.Date)) + " "+ Extensions.PersianMonthName((byte)persianCalendar.GetYear(item.Date)) + "</b><br/> {point.y}{series.name} </div>,";
            }
            if(PriceCHart.Length>2)
            ViewBag.PriceCHart = PriceCHart.Substring(0, PriceCHart.Length-1);
            if (DateChart.Length > 2)
                ViewBag.DateChart = DateChart.Substring(0, DateChart.Length - 1);
            if (DesChart.Length > 2)
                ViewBag.DesChart = DesChart.Substring(0, DesChart.Length - 1);
            return retVal;
        }

        #endregion

        #region GetRateProduct

        private void GetRateProduct(int productId)
        {
            var product = db.Products.Find(productId);
            ViewBag.RateProduct = product.ProductRates.Count > 0 ? product.ProductRates.Sum(x => x.Rate) / product.ProductRates.Count() : 0;
            ViewBag.CountRateProduct = "امتیاز " + ViewBag.RateProduct + " از 5 از مجموع " + product.ProductRates.Count() + " نظر";
        }

        #endregion

    }

}