﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebUI.Models;
using ServicePattern;
using Domain;
using Service.Identity;

namespace WebUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //===========================//
        //--------- MANAGERS ----------
        //===========================//

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ServiceUser _userService = new ServiceUser();


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
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        //===========================//
        //--------- LOGIN ----------
        //===========================//

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AccountViewModels.LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    ModelState.AddModelError("", "Connected.");
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //===========================//
        //--------- LOG OUT ---------
        //===========================//

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //===========================//
        //--------- REGISTER --------
        //===========================//

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(AccountViewModels.RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                IdentityResult result;

                // Switch on Selected Account type
                switch (model.AccountType)
                {
                    // Volunteer Account type selected:
                    case Domain.EAccountType.Patient:
                        {
                            // create new volunteer and map form values to the instance
                            Patient v = new Patient { UserName = model.Email, Email = model.Email,address=model.address,firstName=model.firstName,lastName=model.lastName };
                            result = await UserManager.CreateAsync(v, model.Password);

                            // Add volunteer role to the new User
                            if (result.Succeeded)
                            {
                               // UserManager.AddToRole(v.Id, EAccountType.Patient.ToString());
                                await SignInManager.SignInAsync(v, isPersistent: false, rememberBrowser: false);
                                // Email confirmation here

                                return RedirectToAction("Index", "Home");
                            }
                            AddErrors(result);
                        }
                        break;

                    // Ngo Account type selected:
                    case EAccountType.Doctor:
                        {
                            // create new Ngo and map form values to the instance
                            Doctor ngo = new Doctor { UserName = model.Email, Email = model.Email, address = model.address, firstName = model.firstName, lastName = model.lastName };
                            result = await UserManager.CreateAsync(ngo, model.Password);

                            // Add Ngo role to the new User
                            if (result.Succeeded)
                            {
                           //     UserManager.AddToRole(ngo.Id, EAccountType.Doctor.ToString());
                                await SignInManager.SignInAsync(ngo, isPersistent: false, rememberBrowser: false);

                                return RedirectToAction("Index", "Home");
                            }
                           // AddErrors(result);
                        }
                        break;
                }

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //===========================//
        //--------- HELPERS ---------
        //===========================//


        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // FOR REFERENCE
        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        // Send an email with this link
        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");


    }
}