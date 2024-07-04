using JWT.Filters;
using JWT.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace JWT.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
    
        [AllowAnonymous]
        [HttpGet]
        public ActionResult LogIn() {
            return View();
        }

       [AllowAnonymous]
       [HttpPost]
        public ActionResult LogIn(UserCred UserInput)
        {
            //create a token and return it to the user
            string userName = "hello";
            string password = "nice";

            //true? create a token and return it
            if(UserInput != null)
            {
                if(UserInput.UserName == userName && UserInput.Password == password) {
                    string token =Authentication.GenerateJWTAuth(userName, password);

                    var cookie = new HttpCookie("token", token)
                    {
                        HttpOnly = true,
                    };
                    Response.Cookies.Add(cookie);

                    Session["ID"] = "ID";
                    Session["username"] = userName;
                }
            }
            return View("Index");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Authenticate(string userName, string password)
        {
            //generate token
            if(userName == null || password == null)
            {
                return Json(new { message = "enter creds" });
            }
            string token = Authentication.GenerateJWTAuth(userName,password);
            return Json(new { token });
        }

        [HttpGet]
        public ActionResult GetData() 
        {
            string Data = DateTime.Now.ToString();
            return Json(Data, JsonRequestBehavior.AllowGet);
            
        }

        [JWTAuth]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            HttpCookie cookie = Request.Cookies["token"];
            HttpCookie cccookkiiee = Request.Cookies["test"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-3);
                Response.Cookies.Add(cookie);
            }
            return View();
        }

        [JWTAuth]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}