using SurveyConfiguratorWeb.Attributes;
using SurveyConfiguratorWeb.ConstantsAndMethods;
using SurveyConfiguratorWeb.Models;
using SurveyConfiguratorWeb.Models.LogIn;
using SurveyConfiguratorWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SurveyConfiguratorWeb.Controllers
{
    public class LogInController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            //check if the user has cookies and is validated and redirect if so
            if (!States.IsAuthenticated)
            {
                return View();
            }
            else
            {
                //return to home page with error pop up 
                //add an error message to view data obj

                return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
            }
        }

        [HttpPost]
        public ActionResult Index(LogInViewModel pUserCredintials)
        {
            //extract user data form the model
            string tEnteredUserName = pUserCredintials.UserName;
            string tEnteredPassword = pUserCredintials.Password;

            //validate and find user credintials using the user name
            string tStoredUserName = WebConfigurationManager.AppSettings[SharedConstants.cTestUserNameSettingsKey];
            if (!string.IsNullOrEmpty(tEnteredUserName) && tStoredUserName.Equals(tEnteredUserName))
            {
                //user creds found
                //user credintials from db
                string tEncryptedPassword = WebConfigurationManager.AppSettings[SharedConstants.cTestPasswordSettingsKey];
                if(!string.IsNullOrEmpty(tEnteredPassword) && tEncryptedPassword.Equals(tEnteredPassword))
                {
                    //successful log in, give tokens in cookies and redirect
                    //access token
                    string tAccessToken = TokenManager.GenerateJWT(tEnteredUserName, false);
                    HttpCookie tAccessTokenCookie = new HttpCookie(SharedConstants.cAccessTokenKey, tAccessToken);

                    //refresh token
                    string tRefreshToken = TokenManager.GenerateJWT(tEnteredUserName, true);
                    HttpCookie tRefreshTokenCookie = new HttpCookie(SharedConstants.cRefreshTokenKey, tRefreshToken)
                    {
                        HttpOnly = true
                    };

                    //add cookies to response
                    Response.AppendCookie(tAccessTokenCookie);
                    Response.AppendCookie(tRefreshTokenCookie);
                    //return to home page

                    //set user authentication state to true
                    States.IsAuthenticated = true;

                    return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
                }
            }
            //user creds not found or not matching password
            //add error message to modelState and return the same view
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (!States.IsAuthenticated)
            {
                //check if the user has cookies and is validated and redirect if so
                return View();
            }
            else
            {
                //return to home page with error pop up 
                //add an error message to view data obj

                return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
            }
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel pUserData)
        {
            //extract user data form the model
            string tEnteredUserName = pUserData.UserName;
            string tEnteredEmail = pUserData.Email;
            string tEnteredPassword = pUserData.Password;

            //validate input
            if (string.IsNullOrEmpty(tEnteredUserName) ||
                string.IsNullOrEmpty(tEnteredEmail)    ||
                string.IsNullOrEmpty(tEnteredPassword))
            {
                //add to viewData container a message to indicate that the user needs to be signed in to perform this action
                //edit modelState ?
                return View();
            }

            //input is valid
            //add user to db

            //send tokens
            string tAccessToken = TokenManager.GenerateJWT(tEnteredUserName, false);
            HttpCookie tAccessTokenCookie = new HttpCookie(SharedConstants.cAccessTokenKey, tAccessToken);

            //refresh token
            string tRefreshToken = TokenManager.GenerateJWT(tEnteredUserName, true);
            HttpCookie tRefreshTokenCookie = new HttpCookie(SharedConstants.cRefreshTokenKey, tRefreshToken)
            {
                HttpOnly = true
            };

            //add cookies to response
            Response.AppendCookie(tAccessTokenCookie);
            Response.AppendCookie(tRefreshTokenCookie);

            //set user authentication state to true
            States.IsAuthenticated = true;

            //return to home page
            return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
        }

        [Authenticated]
        [HttpGet]
        public ActionResult LogOut()
        {
            //delete access token cookie if found
            if (Request.Cookies[SharedConstants.cAccessTokenKey] != null)
            {
                //delete cookie by setting its refresh time in the past
                Response.Cookies[SharedConstants.cAccessTokenKey].Expires = DateTime.UtcNow.AddDays(SharedConstants.cCookiesForceDeletionTimeInDays);
            }

            //delete refresh token cookie if found and invalidate it 
            if (Request.Cookies[SharedConstants.cRefreshTokenKey] != null)
            {
                //delete cookie by setting its refresh time in the past 
                Response.Cookies[SharedConstants.cRefreshTokenKey].Expires = DateTime.UtcNow.AddDays(SharedConstants.cCookiesForceDeletionTimeInDays); 
                //invalidate cookie 

            }

            //set user authentication state to true
            States.IsAuthenticated = false;

            //send user to home page
            return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
        }
    }
}