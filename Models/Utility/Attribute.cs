using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kaspid.Models.Utility
{
    public class AccessFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            // TODO: Add your action filter's tasks here

            // Log Action Filter call
            using (KaspidModel db = new KaspidModel())
            {

                string Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string Action = string.Concat(filterContext.ActionDescriptor.ActionName,"(Logged By: Custom Action Filter)");
                if (SiteUtility.UserIsAdminL2)
                    if (!db.UserModules.Any(p => p.User.Id == SiteUtility.UserId && p.Module.URL == Controller))
                    {
                        var url = new UrlHelper(filterContext.RequestContext);
                        var loginUrl = url.Content("~/Error/NoAccess");
                        filterContext.HttpContext.Response.Redirect(loginUrl);
                    }
            }
        }
    }
    public class PhoneAttribute : RegularExpressionAttribute
    {
        public PhoneAttribute()
            : base(@"[0-9-]+")
        {
            this.ErrorMessage = "تلفن نامعتبر است";
        }
    }
    public class MobileAttribute : RegularExpressionAttribute
    {
        public MobileAttribute()
           :base(@"09(1[0-9]|3[1-9]|2[1-9])-?[0-9]{3}-?[0-9]{4}")
            // : base(@"09(\d|\-)+\d")
        {
            this.ErrorMessage = "شماره همراه نامعتبر است";
        }
    }
    public class EmailAttribute : RegularExpressionAttribute
    {
        public EmailAttribute()
            : base(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}")
        {
            this.ErrorMessage = "پست الکترونیک نامعتبر است";
        }
    }
   
}