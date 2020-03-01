using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WAD.CW2_WebApp._5529
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{culture}/{controller}/{action}/{id}/",
                defaults: new {culture="en", controller = "Authentication", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
