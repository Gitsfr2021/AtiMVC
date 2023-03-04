using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace Kaspid.App_Start
{
    public class CmsUrlConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            Kaspid.Models.DalEntities db = new Kaspid.Models.DalEntities();
            if (values[parameterName] != null)
            {
                var permalink = values[parameterName].ToString();
                if (permalink.EndsWith("/"))
                    permalink = permalink.Substring(0, permalink.Length - 1);
                return db.CmsPages.Any(p => p.Url.ToLower() == permalink.ToLower());
            }
            return false;
        }
    }

    public class ValidControllerAction : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var action = values["action"] as string;
            var controller = values["controller"] as string;
            string first = controller.Substring(0, 1).ToUpper();
            string last = controller.Substring(1, controller.Length - 1);
            controller = first + last;
            var controllerFullName = string.Format("Kaspid.Controllers.{0}Controller", controller);

            var cont = Assembly.GetExecutingAssembly().GetType(controllerFullName);
            return cont != null;
        }
    }
    public class ValidGroupAddress : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            List<string> urls = new List<string> { "services", "products", "news" };
            var action = values["action"] as string;
            var controller = values["controller"] as string;
            if (urls.Contains(controller.ToLower()))
                return true;
            else
                return false;
        }
    }
    public class LogoutConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            Kaspid.Models.DalEntities db = new Kaspid.Models.DalEntities();
            if (values[parameterName] != null)
            {
                var permalink = values[parameterName].ToString();
                if (permalink.EndsWith("/"))
                    permalink = permalink.Substring(0, permalink.Length - 1);
                return db.CmsPages.Any(p => p.Url.ToLower() == permalink.ToLower());
            }
            return false;
        }
    }
}