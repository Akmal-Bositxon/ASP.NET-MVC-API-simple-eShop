using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace WAD.CW2_WebApp._5529
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        //protected void Application_BeginRequest(Object source, EventArgs e)
        //{
        //    HttpContextBase currentContext = new HttpContextWrapper(HttpContext.Current);
        //    RouteData routeData = RouteTable.Routes.GetRouteData(currentContext);
        //    var app = (HttpApplication)source;
        //    var language = routeData.Values["lang"].ToString();

        //    if (language != "ru" || language!="api")
        //    {
        //        language = "en";
        //    }

        //    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(language);
        //    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        //}

        protected void Application_AcquireRequestState()
        {
            try
            {
                HttpCookie authCookie = Request.Cookies.Get("UserLoginData");
                if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value) && FormsAuthentication.Decrypt(authCookie.Value) != null)
                {
                    var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    var userName = ticket.Name;
                    Session["User"] = userName;
                }
                else
                {
                    Session["User"] = null;
                }
            }
            catch
            {

            }
        }
    }
}
