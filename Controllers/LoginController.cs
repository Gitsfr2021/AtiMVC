using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Kaspid.Models;
using Kaspid;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using Kaspid.Models.Utility;
using WebMarkupMin.AspNet4.Mvc;


namespace Kaspid.Controllers
{
    public class LoginController : InsideController
    {
        #region Fields

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        #endregion

        #region Ctor

        public LoginController() { }
        public LoginController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        #endregion

        #region Properties

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().Get<ApplicationUserManager>(); }
            private set
            {
                _userManager = value;
            }
        }

        #endregion

        #region Methods

        #region CreateAdmin

        private async Task CreateAdminAsync()
        {
            using (DalEntities db = new DalEntities())
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var user = new UserInfo { UserName = "admin", Email = "info@domain.com", UserType = (byte)AllEnums.UserType.Admin };
                var result = await UserManager.CreateAsync(user, "password");

                if (!roleManager.RoleExists("Admin"))
                {
                    var role = new IdentityRole();
                    role.Name = "Admin";
                    roleManager.Create(role);


                }
                if (result.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }

            }
        }
        private void ChangePassword()
        {
            using (DalEntities db = new DalEntities())
            {
                UserInfo cUser = UserManager.FindByName("admin");
                cUser.AccessFailedCount = 0;
                cUser.IsLockOut = false;

                var Rresult = UserManager.RemovePassword(cUser.Id);
                if (Rresult.Succeeded)
                {
                    var result2 = UserManager.AddPassword(cUser.Id, "7a8?1q");
                    if (result2.Succeeded)
                    {
                        db.SaveChanges();
                    }
                }

            }
        }
        #endregion

        #region RedirectUser

        private ActionResult RedirectUser(string returnUrl)
        {
            string role = UserManager.GetRoles(SiteUtility.UserInfo.Id).FirstOrDefault();
            if (role == "Admin"|| role == "adminl2")
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return Redirect("/admin?op=login");
            }
            else
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return Redirect("/profile?op=login");
            }
        }

        #endregion

        #region Index

        [MinifyHtml]
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            //ChangePassword();
           // CreateAdminAsync();
            if (SiteUtility.UserIsLogin)
            {
                return RedirectUser(returnUrl);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect("/login/");
                        }
                        else
                        {
                            return Redirect(returnUrl);
                        }
                    }
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("","کاربردر حال حاضر غیرفعال است");
                    return View(model);
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = true });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("","نام کاربری یا کلمه عبور نامعتبر است");
                    return View(model);
            }
        }

        [HttpPost]
        public string Popup(string username, string password, string returnUrl)
        {
            var result = SignInManager.PasswordSignIn(username, password, false, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    Redirect(returnUrl);
                    return "1";
                case SignInStatus.LockedOut:
                    return "کاربردر حال حاضر غیرفعال است";
                case SignInStatus.Failure:
                default:
                    return "نام کاربری و کلمه عبور نامعتبر است";

            }
       
        }

        #endregion

        #region Logout

        [Route("Logout")]
        [Authorize]
        public ActionResult logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();
            return RedirectToAction("Index","Login");
        }

        #endregion

        #endregion
    }
}