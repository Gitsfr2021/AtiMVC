using Kaspid.Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kaspid.Models
{
    [Table("ProductGroup")]
    public class ProductGroup
    {
        #region Ctor

        public ProductGroup()
        {
            Childs = new HashSet<ProductGroup>();
        }

        #endregion

        #region Properties

        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "{0} را وارد نمایید")]
        public string Title { get; set; }

        [Display(Name = "عنوان آدرس")]
        [Required(ErrorMessage = "{0} را وارد نمایید")]
        public string TitleURL { get; set; }

        [Display(Name = "عنوان صفحه")]
        [Required(ErrorMessage = "{0} را وارد نمایید")]
        public string PageTitle { get; set; }

        public string PageDescription { get; set; }
        public string PageDCSubject { get; set; }

        [UIHint("ImageBrowser")]
        [Display(Name = "تصویر")]
        public string Picture { get; set; }

        [UIHint("ImageBrowser")]
        [Display(Name = "تصویر")]
        public string Picture1 { get; set; }

        [UIHint("ImageBrowser")]
        [Display(Name = "تصویر")]
        public string offerPic { get; set; }

        [UIHint("ImageBrowser")]
        [Display(Name = "آیکن")]
        public string Icon { get; set; }

        [Column(TypeName = "ntext")]
        [UIHint("KendoEditor")]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        public string Color { get; set; }
        public bool IsDeleted { get; set; } = false;

        public bool isOffer { get; set; } = false;

        public int ShowOrder { get; set; }
        public byte Status { get; set; } = (byte)AllEnums.Status.hide;
        public string Culture { get; set; } = "fa-IR";
        public bool HasColor { get; set; } = false;
        public bool HasSize { get; set; } = false;

        #endregion

        #region Relations

        [UIHint("TreeDropDown")]
        public int? ParentId { get; set; }
        public virtual ProductGroup Parent { get; set; }
        public virtual ICollection<ProductGroup> Childs { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<DynamicField> DynamicFields { get; set; }
        public virtual ICollection<ProductFeatureFaq> ProductFeatureFaqs{ get; set;}

        public virtual Brand Brand { set; get; }
        public int? BrandId { set; get; }

        #endregion
    }
}
