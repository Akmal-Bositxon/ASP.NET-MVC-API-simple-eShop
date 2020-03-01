using System.Web;
using System.Web.Mvc;

namespace WAD.CW2_WebApp._5529
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
