using Kaspid.Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;

namespace Kaspid.Models
{
    [Table("Product")]
    public class Product
    {
        #region Ctor

        public Product()
        {
            this.ProductColors = new HashSet<ProductColor>();
            this.ProductGalleries = new HashSet<ProductGallery>();
            this.Comments = new HashSet<Comment>();
            this.ProductRates = new HashSet<CommentRate>();
            this.Favorites = new HashSet<Favorites>();
            this.ProductAmazings = new HashSet<ProductAmazing>();
        }

        #endregion

        #region Properties

        [Key]
        public int Id { get; set; }

        [StringLength(200)]
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0} را وارد نمایید")]
        public string Title { get; set; }
        public string entitle { get; set; }

        [Required]
        [Display(Name ="کد")]
        public string Code { get; set; }


        [UIHint("FileBrowser")]
        [StringLength(500)]
        public string AttachedFile { get; set; }


        [Display(Name = "عنوان صفحه")]
        [Required(ErrorMessage = "{0} را وارد نمایید")]
        public string PageTitle { get; set; }

        [Required]
        [Display(Name = "عنوان آدرس")]
        public string TitleURL { get; set; }

        [StringLength(200)]
        public string PageDescription { get; set; }

        public string PageDCSubject { get; set; }

        [UIHint("ImageBrowser")]
        [StringLength(500)]
        public string Picture { get; set; }


        [UIHint("KendoEditor")]
        [Column(TypeName = "ntext")]
        public string Feature { get; set; }


        [UIHint("KendoEditor")]
        [Column(TypeName = "ntext")]
        public string HtmlBody { get; set; }

        [UIHint("KendoEditor")]
        [Column(TypeName = "ntext")]
        public string Help { get; set; }

        public int? ShowOrder { get; set; } 

        public byte Status { get; set; } = (byte)AllEnums.Status.hide;

        public bool ShowInMain { get; set; } = false;
        public bool ShowInNewst { get; set; } = false;
        public bool showinfilter { get; set; } = false;
        public bool specialGroup { get; set; } = false;

        public bool MenGift { get; set; } = false;
        public bool WomanGift { get; set; } = false;
        public bool CartGift { get; set; } = false;
        public bool BocGift { get; set; } = false;

        [StringLength(5)]
        public string Culture { get; set; } = "fa-IR";

        public DateTime DateX { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        public int? Point{ get; set; }

        public long Weight { get; set; } = 0;
        public string Warranty { get; set; }

        public int? Viewcount { set; get; } = 0;

        public int? LikeCount { set; get; } = 0;
        public int? DisLikeCount { set; get; } = 0;

        /// <summary>
        /// جنسیت
        /// </summary>
        //[Display(Name = "وضعیت ارسال")]
        //[Required(ErrorMessage = "{0} را وارد نمایید")]
        public AllEnums.OrderStatus OrderStatus { get; set; }

        #endregion

        #region Store

        [NotMapped]
        public ProductColor SelectedProductColor
        {
            get
            {
                if (ProductColors.Where(x => x.Status == 2 && x.IsDeleted == false && x.HasStock == true).OrderBy(p => p.Price).Count() > 0)
                    return this.ProductColors.Where(x => x.Status == 2 && x.IsDeleted == false && x.HasStock == true).OrderBy(p => p.Price).FirstOrDefault();
                else if (ProductColors.Where(x => x.Status == 2 && x.IsDeleted == false).OrderByDescending(p => p.Price).Count() > 0)
                    return this.ProductColors.Where(x => x.Status == 2 && x.IsDeleted == false).OrderBy(p => p.Price).FirstOrDefault();
                return null;
            }
        }


        [NotMapped]
        public int SumSellcount
        {
            get
            {
                if (ProductColors.Count > 0)
                    return this.ProductColors.Sum(x => x.SellCount);
                return 0;
            }
        }

        [NotMapped]
        public int IsStock
        {
            get
            {
                if (ProductColors.Count > 0)
                    return this.ProductColors.Sum(x=>x.Stock)- this.ProductColors.Sum(x => (x.SellCount == null ? 0 : x.SellCount)) - this.ProductColors.Sum(x=>(x.WarningPoint==null ? 0: x.WarningPoint));
                return 0;
            }
        }

        [NotMapped]
        public long Price
        {
            get
            {
                if (this.SelectedProductColor!=null)
                    return this.SelectedProductColor.Price ;
                return 0;
            }
        }


        [NotMapped]
        public int Discount
        {
            get
            {
                if (this.SelectedProductColor != null)
                    return Convert.ToInt32 (this.SelectedProductColor.Discount);
                return 0;
            }
        }

        [NotMapped]
        public long FinalPrice
        {
            get
            {
                return Convert.ToInt32(this.Price - ((float)this.Price * this.Discount / 100));
            }
        }
        #endregion

        #region Relations

        [UIHint("TreeDropDown")]
        [Display(Name = "گروه")]
        [Required(ErrorMessage = "{0} را انتخاب نمایید")]
        public int ProductGroupId { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        public virtual ICollection<ProductGallery> ProductGalleries { get; set; }
        public virtual ICollection<ProductColor> ProductColors{ get; set; }
        public virtual ICollection<Comment> Comments{ get; set; }
        public virtual ICollection<CommentRate> ProductRates{ get; set; }
        public virtual ICollection<Favorites> Favorites{ get; set; }
        public virtual ICollection<ProductAmazing> ProductAmazings{ get; set; }
        [Required(ErrorMessage = "برند را انتخاب نمایید")]
        public int? BrandId { get; set; }
        public virtual Brand Brand { get; set; }

        //[Required(ErrorMessage = "نحوه ارسال را انتخاب نمایید")]
        //public int? SendTypeId { get; set; }
        public virtual SendType SendType { get; set; }
        public virtual ProductType ProductType { set; get; }
        public int? ProductTypeId { set; get; }

        #endregion
    }
}
