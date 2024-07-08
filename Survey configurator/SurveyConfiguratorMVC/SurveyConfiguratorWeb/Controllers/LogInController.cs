using QuestionServices;
using SharedResources;
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
        private static string cInvalidUserErrorKey = "InvalidUserCredintails";

        /// <summary>
        /// responsible for the Authentication part of the app
        /// issues and deletes cookies containing JWTs for the 
        /// authentication proccess
        /// </summary>

        /// <summary>
        /// gets the login view to let the user sign in to the app
        /// </summary>
        /// <returns>returns the view for the login page</returns>
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                //check if the user has cookies and is validated and redirect if so
                if (!States.IsAuthenticated)
                {
                    return View();
                }
                else
                {
                    //return to home page with error pop up 
                    TempData[SharedConstants.cMessageKey] = GlobalStrings.UserAlreadySignedInError;
                    return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController);
            }
        }

        /// <summary>
        /// sign the user in using the received credintials,
        /// match the credintials against the database to check for 
        /// their correctness, if the user is valid create access
        /// and refresh tokens and send them in cookies
        /// </summary>
        /// <param name="pUserCredintials">object with entered user credintials</param>
        /// <returns>view to the home page if the user is correct</returns>
        [HttpPost]
        public ActionResult Index(LogInViewModel pUserCredintials)
        {
            try
            {
                //extract user data form the model
                string tEnteredUserName = pUserCredintials.UserName;
                string tEnteredPassword = pUserCredintials.Password;

                //validate and find user credintials using the user name
                string tStoredUserName = WebConfigurationManager.AppSettings[SharedConstants.cTestUserNameSettingsKey];
                if (!string.IsNullOrEmpty(tEnteredUserName) && tStoredUserName.Equals(tEnteredUserName))
                {
                    //user credintials found
                    string tEncryptedPassword = WebConfigurationManager.AppSettings[SharedConstants.cTestPasswordSettingsKey];
                    if (!string.IsNullOrEmpty(tEnteredPassword) && tEncryptedPassword.Equals(tEnteredPassword))
                    {
                        //add cookies to the response
                        SetCookies(tEnteredUserName);

                        //set user authentication state to true
                        States.IsAuthenticated = true;

                        return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
                    }
                }
                //user creds not found or not matching password
                //add error message to modelState and return the same view
                ModelState.AddModelError(cInvalidUserErrorKey, GlobalStrings.InvalidUserCredintialsError);
                return View();
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController);
            }
        }

        /// <summary>
        /// gets the Register view to let the user sign up to the app
        /// </summary>
        /// <returns>returns the view for the register page</returns>
        [HttpGet]
        public ActionResult Register()
        {
            try
            {
                if (!States.IsAuthenticated)
                {
                    //check if the user has cookies and is validated and redirect if so
                    return View();
                }
                else
                {
                    //return to home page with error pop up 
                    TempData[SharedConstants.cMessageKey] = GlobalStrings.UserAlreadySignedInError;
                    return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
                }
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController);
            }
        }

        /// <summary>
        /// validate the user input and check if no other user
        /// has exactly matching credintials, if the inputs are valid
        /// and the user is unique send cookies with tokens and redirect to home page
        /// </summary>
        /// <param name="pUserData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(RegisterViewModel pUserData)
        {
            try
            {
                //extract user data form the model
                var tEnteredUserName = pUserData.UserName;
                var tEnteredEmail = pUserData.Email;
                var tEnteredPassword = pUserData.Password;

                //check if the user is unique, if true
                //add user to db

                //add cookies to the response
                SetCookies(tEnteredUserName);

                //set user authentication state to true
                States.IsAuthenticated = true;

                //return to home page
                return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController);
            }
        }

        /// <summary>
        /// logs the user out of the app, the user needs to be signed
        /// in first.
        /// Invalid the user's refresh token and 
        /// remove the user cookies by setting the expire time in the past
        /// then redirect the user to the home page
        /// </summary>
        /// <returns>view to the home page</returns>
        [Authenticated]
        [HttpGet]
        public ActionResult LogOut()
        {
            try
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
                    string tRefreshToken = Request.Cookies[SharedConstants.cRefreshTokenKey].Value;

                    //get the token id 
                    string tTokenId = TokenManager.GetTokenId(tRefreshToken);
                    //invalidate token
                    AuthenticationServices.AddToken(tTokenId);
                    //delete cookie by setting its refresh time in the past 
                    Response.Cookies[SharedConstants.cRefreshTokenKey].Expires = DateTime.UtcNow.AddDays(SharedConstants.cCookiesForceDeletionTimeInDays);
                }

                //set user authentication state to true
                States.IsAuthenticated = false;

                //send user to home page
                return RedirectToAction(SharedConstants.cQuestionsIndexAction, SharedConstants.cQuestionsController);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
                return RedirectToAction(SharedConstants.cErrorPageAction, SharedConstants.cErrorController);
            }
        }

        #region utility functions
        /// <summary>
        /// creates cookies containing access and refrish tokens and 
        /// adds the them to the response sent to the user
        /// </summary>
        /// <param name="pUserName">user name to add to token</param>
        private void SetCookies(string pUserName)
        {
            try
            {
                //successful log in, give tokens in cookies and redirect
                //access token
                string tAccessToken = TokenManager.GenerateJWT(pUserName, false);
                HttpCookie tAccessTokenCookie = new HttpCookie(SharedConstants.cAccessTokenKey, tAccessToken);

                //refresh token
                string tRefreshToken = TokenManager.GenerateJWT(pUserName, true);
                HttpCookie tRefreshTokenCookie = new HttpCookie(SharedConstants.cRefreshTokenKey, tRefreshToken)
                {
                    HttpOnly = true
                };

                //add cookies to response
                Response.AppendCookie(tAccessTokenCookie);
                Response.AppendCookie(tRefreshTokenCookie);
            }
            catch (Exception ex)
            {
                UtilityMethods.LogError(ex);
            }
        }


        #endregion
    }
}