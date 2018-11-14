using Mcf.Attributes;
using McF.Business;
using McF.Contracts;
using McF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace McF.Controllers
{    
    public class HomeController : Controller
    {
        private ICommonService commonservice;
        public HomeController(ICommonService commonservice)
        {
            this.commonservice = commonservice;
        }
        [AuthorizeUser]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeUser]
        public ActionResult Dashboard()
        {
            return View();
        }

        [AuthorizeUser]        
        public ActionResult AddDataSource()
        {
            return View();
        }
        
        public ActionResult Login(string returnURL)
        {
            var userinfo = new LoginViewModel();
            try
            {
                // We do not want to use any existing identity information
                EnsureLoggedOut();
                // Store the originating URL so we can attach it to a form field
                userinfo.ReturnURL = returnURL;
                return View(userinfo);
            }
            catch
            {
                throw;
            }
        }        
        public ActionResult Register(string returnURL)
        {
            var registerInfo = new RegisterViewModel();

            try
            {
                return View(registerInfo);
            }
            catch
            {
                throw;
            }
        }        
        public ActionResult ResetPassword(string returnURL)
        {
            var resetPasswordInfo = new ResetPasswordViewModel();

            try
            {
                return View(resetPasswordInfo);
            }
            catch
            {
                throw;
            }
        }
        //GET: EnsureLoggedOut
        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (Request.IsAuthenticated)
                Logout();
        }
        public ActionResult Logout()
        {
            try
            {
                // First we clean the authentication ticket like always
                //required NameSpace: using System.Web.Security;
                FormsAuthentication.SignOut();

                // Second we clear the principal to ensure the user does not retain any authentication
                //required NameSpace: using System.Security.Principal;
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

                Session.Clear();
                System.Web.HttpContext.Current.Session.RemoveAll();

                // Last we redirect to a controller/action that requires authentication to ensure a redirect takes place
                // this clears the Request.IsAuthenticated flag since this triggers a new request
                return RedirectToAction("Login", "Home");
            }
            catch
            {
                throw;
            }

        }
        //GET: RedirectToLocal
        private ActionResult RedirectToLocal(string returnURL = "")
        {
            try
            {
                // If the return url starts with a slash "/" we assume it belongs to our site
                // so we will redirect to this "action"
                if (!string.IsNullOrWhiteSpace(returnURL) && Url.IsLocalUrl(returnURL))
                    return Redirect(returnURL);

                // If we cannot verify if the url is local to our host we redirect to a default location
                return RedirectToAction("Dashboard", "Home");
            }
            catch
            {
                throw;
            }
        }
        //GET: SignInAsync
        private void SignInRemember(string userName, bool isPersistent = false)
        {
            // Clear any lingering authencation data
            FormsAuthentication.SignOut();

            // Write the authentication cookie
            FormsAuthentication.SetAuthCookie(userName, isPersistent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel entity)
        {
            try
            {

                var isValidUser = commonservice.ValidateUser(entity.Username, entity.Password);
                if (isValidUser)
                {
                    //Login Success
                    //For Set Authentication in Cookie (Remeber ME Option)
                    SignInRemember(entity.Username, entity.RememberMe);
                    //Set A Unique ID in session
                    Session["UserID"] = entity.Username;
                    // If we got this far, something failed, redisplay form
                    // return RedirectToAction("Index", "Dashboard");
                    return RedirectToLocal();
                }
                else
                {
                    //Login Fail
                    TempData["ErrorMSG"] = "Access Denied! Invalid Credentials";
                    return View(entity);
                }
            }

            catch
            {
                throw;
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UserLoginData userData = new UserLoginData() { UserName = entity.Email, Password = entity.Password, IsActive = true, UserId = entity.Name };
                    commonservice.PopulateUser(userData);
                    return RedirectToAction("Dashboard", "Home");
                }
            }

            catch
            {
                throw;
            }
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel entity)
        {
            try
            {
                if (ModelState.IsValid)
                {   
                    commonservice.ResetPassword(entity.Email, entity.Password, entity.NewPassword);
                    return RedirectToAction("Dashboard", "Home");
                }
            }

            catch
            {
                TempData["ErrorMSG"] = "Invalid Details";
            }
            return View();

        }

    }

}