using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ApiErorr
/// </summary>
/// 
namespace Kaspid.Models.Utility
{
    public class ApiErorr
    {

        public enum Erorr
        {
            Ok = 200,
            UserNameNotValied = 100,
            WrongJsonInputFormat = 101,
            WrongJsonInputParam = 102,
            MobileIsRepeat = 103,
            SystemError = 104,
            RepeatItem = 105,
            InvaliedUser = 106,
            ItemNotFound = 107,
            StockIsLow = 108,
            SmsCode = 109,
            EmailIsRepeat = 110,
            ClockIsFull = 111,
            DontAllow = 112,
            FailedEmail=113,
            FailedDirectory=114,
            ParamsEmpty=115,
            NotStock=116,
           
        }
        public static string OperationPersianName(Erorr Erorr)
        {
            string retVal = string.Empty;
            switch (Erorr)
            {
                case Erorr.Ok:
                    retVal = "عملیات با موفقیت انجام شد";
                    break;
                case Erorr.WrongJsonInputFormat:
                    retVal = "فرمت جیسان ورودی معتبر نیست";
                    break;
                case Erorr.WrongJsonInputParam:
                    retVal = "پارامتر مورد نظریافت نشد";
                    break;
                case Erorr.MobileIsRepeat:
                    retVal = "نام کاربری تکراری است";
                    break;
                case Erorr.SystemError:
                    retVal = "خطای سیستم";
                    break;
                case Erorr.UserNameNotValied:
                    retVal = "نام کاربری یا کلمه عبور اشتباه است";
                    break;
                case Erorr.RepeatItem:
                    retVal = "این آیتم قبلا درسیستم ثبت شده است";
                    break;
                case Erorr.InvaliedUser:
                    retVal = "لطفا مجددا لاگین نمایید.سشن شما به پایان رسیده است یا نامعتبراست";
                    break;
                case Erorr.ItemNotFound:
                    retVal = "آیتم مورد نظر یافت نشد";
                    break;
                case Erorr.StockIsLow:
                    retVal = "موجودی این محصول کافی نیست";
                    break;
                case Erorr.SmsCode:
                    retVal = "کدفعال سازی معتبر نیست";
                    break;
                case Erorr.EmailIsRepeat:
                    retVal = "پست الکترونیک تکراری است";
                    break;
                case Erorr.ClockIsFull:
                    retVal = "این ساعت یا پر شده یا در حال استفاده توسط شخصی دیگری است";
                    break;
                case Erorr.DontAllow:
                    retVal = "شما در حال حاضر مجوز پرداخت و ادامه رزرو را ندارید";
                    break;
                case Erorr.FailedEmail:
                    retVal = "درحال حاضر ایمیل ارسال نخواهدشد";
                    break;
                case Erorr.FailedDirectory:
                    retVal = "پوشه کاربر ایجاد نشد";
                    break;
                case Erorr.ParamsEmpty:
                    retVal = "پرکردن اطلاعات الزامی است";
                    break;
                case Erorr.NotStock:
                    retVal = "یکی از محصولات در خواستی ناموجود است";
                    break;
            }
            return retVal;
        }

    }
    public class ResultType
    {
        public enum Status { Ok = 1, Error = 2 }
        public static string StatusPersianName(Status Status)
        {
            string retVal = string.Empty;
            switch (Status)
            {
                case Status.Ok:
                    retVal = "Ok";
                    break;
                case Status.Error:
                    retVal = "Error";
                    break;
            }
            return retVal;
        }

    }
}