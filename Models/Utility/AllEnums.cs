using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Kaspid.Models.Utility
{
    public class AllEnums
    {
        #region Table
        public enum ColorType
        {
            [Display(Name = "color1")]
            Pink = 1,
            [Display(Name = "color2")]
            Green = 2,
            [Display(Name = "color3")]
            Blue = 3,
            [Display(Name = "color4")]
            DarkBlue = 4,
            [Display(Name = "color5")]
            Purple = 5,
            [Display(Name = "color6")]
            Orange = 6,
        }
        public enum Status
        {
            [Display(Name = "نمایش")]
            Show = 2,
            [Display(Name = "عدم نمایش")]
            hide = 1,
        }
        public enum ItemStatus
        {
            [Display(Name = "تاییدشده")]
            Show = 2,
            [Display(Name = "درانتظارتایید")]
            Waiting = 1,
            [Display(Name = "ردشده")]
            hide = 3,
            [Display(Name = "بازبینی مجدد")]
            Review = 4,
        }
        public enum StoreType
        {
            [Display(Name = "فروشگاه")]
            Store = 1,
            [Display(Name = "رزرواسیون")]
            Rezerve = 2,
        }
        public enum AdminStatus
        {
            [Display(Name = "<span class='activeItem'>نمایش</span>")]
            Show = 2,
            [Display(Name = "<span class='deactiveItem'>عدم نمایش</span>")]
            hide = 1,
        }
        public enum ReadStatus
        {
            [Display(Name = "تایید شده")]
            Show = 2,
            [Display(Name = "عدم تایید")]
            hide = 1,
        }
        public enum AdminReadStatus
        {
            [Display(Name = "<span class='activeItem'>تایید شده</span>")]
            Show = 2,
            [Display(Name = "<span class='deactiveItem'>عدم تایید</span>")]
            hide = 1,
        }
        public enum Operation
        {
            [Display(Name = "درج")]
            Insert = 1,
            [Display(Name = "بروز رسانی")]
            Update = 2,
            [Display(Name = "حذف")]
            Delete = 3,
            [Display(Name = "ورود به سایت")]
            Login = 4,
            [Display(Name = "خروج از سایت")]
            Logout = 5,
            [Display(Name = "تغییر کلمه عبور")]
            ChangePassword = 6
        }
        #endregion
        #region Poll

        public enum PollOptionFiledType
        {
            [Display(Name = "TextBox")]
            TextBox = 1,
            [Display(Name = "RadioButton")]
            RadioButton = 2,
            [Display(Name = "CheckBox")]
            CheckBox = 3,

        }

        #endregion
        #region User
        public enum UserType
        {
            [Display(Name = "مدیر")]
            Admin = 1,
            [Display(Name = "مدیران سطح 2")]
            AdminLevelTow = 2,
            [Display(Name = "کاربران")]
            User = 3,
            [Display(Name = "فروشگاه")]
            Provider = 4
        }

        public enum SiteRoles
        {
            admin = 1,
            user = 2,
            adminl2 = 3,
            creator = 4
        };
        #endregion
        #region Product
        public enum ProductFeatureType
        {
            [Display(Name = "جنس رویه")]
            Material = 1,
            [Display(Name = "شلف")]
            Shelf = 2,
            [Display(Name = "لوازم جانبی")]
            sideProduct = 3,
            [Display(Name = "کابینت")]
            cabinet = 4,
            //[Display(Name = "کابینت‌های دیواری")]
            //wallcabinet = 5,
        }
        #endregion
        #region Banner
        public enum BannerPosition
        {
            [Display(Name = "صفحه اصلی (433*1200)")]
            MainPga = 1,
            [Display(Name = "صفحه داخلی")]
            InnerPage = 2,
            [Display(Name = "بنر تبلیغاتی پایین اسلایدر (271*380) ")]
            AdsBotSlider = 3,
            [Display(Name = "بنر تبلیغاتی پایین محصولات شگفت انگیز (185*285) ")]
            AdsBotAmazing = 4,
            [Display(Name = "بنر تبلیغاتی پایین پرفروش ترین محصولات  (250*586) ")]
            AdsBotBestSell = 4,
            [Display(Name = "بنر تبلیغاتی پایین جدید ترین محصولات  (250*586) ")]
            AdsBotNewestPro = 5,
            [Display(Name = "بنر تبلیغاتی لیست محصولات  (197*282) ")]
            ProductGroup = 6,

        }

        public enum AdsPosition
        {

            [Display(Name = "بنر تبلیغاتی پایین صفحه خبر ها ")]
            AdsFullBlog = 1,
            [Display(Name = "بنر تبلیغاتی پایین صفحه آموزش ها ")]
            AdsFullLearn = 2,
            [Display(Name = "بنر تبلیغاتی صفحه اصلی-بزرگ سمت چپ پایین ")]
            HomeLeft = 3,
            [Display(Name = "بنر تبلیغاتی صفحه اصلی-بزرگ سمت راست پایین ")]
            Homebottomright = 4,
            [Display(Name = "بنر تبلیغاتی صفحه اصلی-کوچک سمت راست بالا چپ ")]
            homerighttopleft = 5,
            [Display(Name = "بنر تبلیغاتی صفحه اصلی-کوچک سمت راست بالا راست ")]
            homerighttopright = 6,
        }

        public enum OrderStatusComment
        {
            [Display(Name = "نظری ندارم")]
            NoIda = 0,
            [Display(Name = "خرید این محصول را توصیه میکنم ")]
            OK = 2,
            [Display(Name = "در مورد خرید این محصول مطمئن نیستم")]
            NoK = 1,
        }

        public enum BannerPageType
        {
            [Display(Name = "صفحه جاری")]
            NoLink = 1,
            [Display(Name = "صفحه جدید")]
            NewPage = 2,
            [Display(Name = "آدرس جدید")]
            NewAddress = 3,
        }
        #endregion
        #region Comment
        public enum ObjectTypeComment
        {
            [Display(Name = "اخبار")]
            News = 1,
            [Display(Name = " مقالات")]
            Blog = 2,
            [Display(Name = "محصولات")]
            Product = 3,
            [Display(Name = "پادکست")]
            Padcast = 4,
            [Display(Name = "ویدئو")]
            Video = 5,
        }

        public enum CommentStatus
        {
            [Display(Name = "بررسی شده")]
            Accepted = 2,
            [Display(Name = "درحال بررسی")]
            InWait = 1,
        }

        #endregion
        #region Project
        public enum ProjectType
        {
            [Display(Name = "دردست اجرا")]
            InExcute = 1,
            [Display(Name = "اجراشده")]
            Done = 2,

        }
        #endregion
        #region Gallery

        public enum ObjectType
        {
            [Display(Name = "محصول")]
            Product = 1,
            [Display(Name = "اخبار")]
            News = 2,

        }
        #endregion
        #region DynamicField
        public enum DynamicFieldType
        {
            [Display(Name = "متنی")]
            Text = 1,
            [Display(Name = "عدد صحیح")]
            Integer = 2,
            [Display(Name = "عدد اعشاری")]
            Floating = 3,
            [Display(Name = "تک آیتمی")]
            SingleItem = 4,
            [Display(Name = "چند آیتمی")]
            MultiItem = 5,
            [Display(Name = "فایل")]
            File = 6,
            [Display(Name = "تاریخ")]
            Date = 7,
            [Display(Name = "بلی/خیر")]
            Boolean = 8
        }
        #endregion
        #region Podcast_video

        public enum Type_Padcast
        {
            [Display(Name = "ویدئو")]
            Video = 1,
            [Display(Name = "پادکست")]
            Padcast = 2,
        }

        #endregion
       
        #region Product Gallery

        public enum TypeProductGallery
        {
            [Display(Name = "ویدئو")]
            Video = 1,
            [Display(Name = "تصویر")]
            image = 2,
        }

        #endregion
        #region Order
        public enum SendType
        {
            [Display(Name = "پیک موتوری")]
            Motor = 1,
            [Display(Name = "پست پیشتاز")]
            Pishtaz = 2,
            [Display(Name = "پست سفارشی")]
            Custome = 3,
            [Display(Name = "تیپاکس")]
            Tipax = 4,
            [Display(Name = "باربری")]
            BarBary = 5,
        }

        public enum PaymentType
        {
            [Display(Name = "پرداخت در محل")]
            Motor = 1,
            [Display(Name = "کارت به کارت")]
            Pishtaz = 2,
            [Display(Name = "درگاه پرداخت")]
            Custome = 3,

        }

        public enum OrderStatus
        {
            [Display(Name = "نامعلوم")]
            unknown = 0,
            [Display(Name = "درحال پرداخت")]
            Paying = 1,
            [Display(Name = "پرداخت شد")]
            Payed = 2,
            [Display(Name = "تحویل داده شد")]
            Delivered = 3,
            [Display(Name = "کنسل شد")]
            Canceled = 4,
            [Display(Name = "درحال ارسال ")]
            Delivering = 5,
            [Display(Name = "درحال تهیه ")]
            InProduce = 6,
            [Display(Name = "آماده ارسال")]
            Ready=7

        }
        #endregion
        public enum Gender
        {
            [Display(Name = "مرد")]
            Man,
            [Display(Name = "زن")]
            Female,
            [Display(Name = "مرد / زن")]
            ManOrFemale
        }

        public enum GenderCoop
        {
            [Display(Name = "مرد")]
            Man,
            [Display(Name = "زن")]
            Female,
        }

        public enum Married
        {
            [Display(Name = "متاهل")]
            Married,
            [Display(Name = "مجرد")]
            Single
        }

        public enum MilitaryServiceStatus
        {
            [Display(Name = "پایان خدمت")]
            TheEndOfService,
            [Display(Name = "معافیت دائم")]
            Exemption,
            [Display(Name = "معافیت تحصیلی")]
            EducationPardon,
            [Display(Name = "درحال انجام")]
            Serving,
            [Display(Name = "خانوم هستم")]
            IsFemale
        }
    }
    public static class EnumHelper<T>
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);

        }

        public static string GetDescription(string value)
        {
            if (value != null)
            {
                T GenericEnum = (T)Enum.Parse(typeof(T), value, true);
                Type genericEnumType = GenericEnum.GetType();
                MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
                if ((memberInfo != null && memberInfo.Length > 0))
                {
                    var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false);
                    if ((_Attribs != null && _Attribs.Count() > 0))
                    {
                        return ((System.ComponentModel.DataAnnotations.DisplayAttribute)_Attribs.ElementAt(0)).Name;
                    }
                }
                return GenericEnum.ToString();
            }
            else return "";
        }


        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        private static string lookupResource(Type resourceManagerProvider, string resourceKey)
        {
            foreach (PropertyInfo staticProperty in resourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (staticProperty.PropertyType == typeof(System.Resources.ResourceManager))
                {
                    System.Resources.ResourceManager resourceManager = (System.Resources.ResourceManager)staticProperty.GetValue(null, null);
                    return resourceManager.GetString(resourceKey);
                }
            }

            return resourceKey; // Fallback with the key name
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes[0].ResourceType != null)
                return lookupResource(descriptionAttributes[0].ResourceType, descriptionAttributes[0].Name);

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }

    }
}