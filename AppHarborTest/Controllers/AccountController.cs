using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AppHarborTest.Models;
using Facebook;
using System.Web.Security;

namespace AppHarborTest.Controllers
{
    public class AccountController : Controller
    {
        private const string LogoffUrl = "http://AppHarborTest-3.apphb.com/";
        private const string RedirectUrl = "http://AppHarborTest-3.apphb.com/Account/OAuth";
        //
        // GET: /Account/LogOn/

        public ActionResult LogOn(string returnUrl)
        {
            var oAuthClient = new FacebookOAuthClient(FacebookApplication.Current) {RedirectUri = new Uri(RedirectUrl)};
            var loginUri = oAuthClient.GetLoginUrl(new Dictionary<string, object> { { "state", returnUrl } });
            return Redirect(loginUri.AbsoluteUri);
        }

        //
        // GET: /Account/OAuth/

        public ActionResult OAuth(string code, string state)
        {
            FacebookOAuthResult oauthResult;
            if (FacebookOAuthResult.TryParse(Request.Url, out oauthResult))
            {
                if (oauthResult.IsSuccess)
                {
                    var oAuthClient = new FacebookOAuthClient(FacebookApplication.Current)
                                          {RedirectUri = new Uri(RedirectUrl)};
                    dynamic tokenResult = oAuthClient.ExchangeCodeForAccessToken(code);
                    string accessToken = tokenResult.access_token;

                    var expiresOn = DateTime.MaxValue;

                    if (tokenResult.ContainsKey("expires"))
                    {
                        DateTimeConvertor.FromUnixTime(tokenResult.expires);
                    }

                    var fbClient = new FacebookClient(accessToken);
                    dynamic me = fbClient.Get("me?fields=id,name");
                    long facebookId = Convert.ToInt64(me.id);

                    InMemoryUserStore.Add(new FacebookUser
                    {
                        AccessToken = accessToken,
                        Expires = expiresOn,
                        FacebookId = facebookId,
                        Name = (string)me.name,
                    });

                    FormsAuthentication.SetAuthCookie(facebookId.ToString(), false);

                    // prevent open redirection attack by checking if the url is local.
                    if (Url.IsLocalUrl(state))
                    {
                        return Redirect(state);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/LogOff/

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            var oAuthClient = new FacebookOAuthClient {RedirectUri = new Uri(LogoffUrl)};
            var logoutUrl = oAuthClient.GetLogoutUrl();
            return Redirect(logoutUrl.AbsoluteUri);
        }

    }
}