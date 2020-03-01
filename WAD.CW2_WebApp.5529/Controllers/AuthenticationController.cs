using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WAD.CW2_WebApp._5529.Models;
using eShopDAL.Repositories;
using eShopDAL.Entities;
using System.Xml.Linq;
using System.Xml.Schema;
using System.IO;
using System.Web.Security;
using Newtonsoft.Json;
using System.Configuration;
using System.Net;
using WAD.CW2_WebApp._5529.Helpers;
using System.Net.Mail;

namespace WAD.CW2_WebApp._5529.Controllers
{
    public class AuthenticationController : BaseController
    {
        public bool isAuthorized;
        // GET: Home
        public ActionResult Index()
        {
            if (Session["User"] != null)
            {
                return RedirectToAction("Index", "Product");
               
            }
            else
            {
                return RedirectToAction("Login");

            }
         
        }


        [HttpGet]
        public ActionResult Logout()
        {
            var httpCookie = HttpContext.Response.Cookies["UserLoginData"];
            if (httpCookie != null)
            {
                httpCookie.Value = string.Empty;
                isAuthorized = false;
            }
      
            return RedirectToAction("Login");
        }
        public ActionResult Success()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            HttpCookie authCookie = Request.Cookies.Get("UserLoginData");
            if (Session["User"] != null)
            {
                RedirectToAction("Index");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = UserExists(model);
            if (user != null)
            {
                var ticket = new FormsAuthenticationTicket(
                    1,
                    string.Format("{0} {1}", user.FirstName, user.LastName),
                    DateTime.Now,
                    DateTime.Now.Add(FormsAuthentication.Timeout),
                    false,
                    user.Id.ToString(),
                    FormsAuthentication.FormsCookiePath
                    );
                var encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var authCookie = new HttpCookie("UserLoginData")
                {
                    Value = encryptedTicket,
                    Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
                };
                var isAuthorizedCookie = new HttpCookie("isAuthorized")
                {
                    Value = encryptedTicket,
                    Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
                };
                HttpContext.Response.Cookies.Set(authCookie);
                HttpContext.Response.Cookies.Set(isAuthorizedCookie);
                isAuthorized = true;
                LogHelper.Info(string.Format("{0} logged in", user.UserName));
                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", Resources.Global.WrongCredentials);
            return View(model);
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegistrationViewModel model)
        {
            if (model.Password != model.ComfirmPassword)
            {
                ModelState.AddModelError("ComfirmPassword", Resources.Global.PasswordsShouldMatch);
            }
            if (UserNameExists(model))
            {
                ModelState.AddModelError("UserName", Resources.Test.UserNameExists);
            }
            if (ModelState.IsValid && ValidateCaptcha())
            {
                var user = MapFromModel(model);
                var repository = new UserRepository();
                repository.Create(user);

                ViewBag.Result = "Email was sent successfully";
                try
                {
                    var mailMessage = new MailMessage("mediaparkbozori@gmail.com", model.Email);
                    mailMessage.Subject = "About Registration from Mediapark";
                    mailMessage.Body ="Hello "+model.FirstName+ "\nCongratulation!!!\n You have successfully registered to Mediapark.uz";

                    var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.Credentials = new NetworkCredential()
                    {
                        UserName = "mediaparkbozori@gmail.com",
                        Password = "00005529"
                    };
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);
                }
                catch 
                {
                   
                }
                LogHelper.Info(string.Format("{0} registered succesfully", user.UserName));
                return RedirectToAction("Success");
            }

            return View(model);
        }
        private User MapFromModel(RegistrationViewModel model)
        {
            return new User
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Password = model.Password
            };
        }
        public User UserExists(LoginViewModel model)
        {
            var reposirtory = new UserRepository();

            return reposirtory.GetAll().FirstOrDefault(u => u.UserName == model.UserName && u.Password == model.Password);
        }
        public bool UserNameExists(RegistrationViewModel model)
        {
            var reposirtory = new UserRepository();

            return reposirtory.GetAll().Any(u => u.UserName.ToLower() == model.UserName.ToLower());
        }
        public class CaptchaResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }
        private bool ValidateCaptcha()
        {
            var response = Request["g-recaptcha-response"];
            //secret that was generated in key value pair
            string secret = ConfigurationManager.AppSettings["reCAPTCHASecretKey"];
            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);
            //when response is false check for the error message
            if (!captchaResponse.Success)
            {
                if (captchaResponse.ErrorCodes.Count <= 0) return false;
                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                        ViewBag.message = "The secret parameter is missing.";
                        break;
                    case ("invalid-input-secret"):
                        ViewBag.message = "The secret parameter is invalid or malformed.";
                        break;
                    case ("missing-input-response"):
                        ViewBag.message = "The response parameter is missing. Please, preceed with reCAPTCHA.";
                        break;
                    case ("invalid-input-response"):
                        ViewBag.message = "The response parameter is invalid or malformed.";
                        break;
                    default:
                        ViewBag.message = "Error occured. Please try again";
                        break;
                }
                return false;
            }
            else
            {
                ViewBag.message = "Valid";
            }
            return true;
        }










    }
}
