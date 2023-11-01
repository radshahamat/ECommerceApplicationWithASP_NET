using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceApplication.Auth
{
    public class AdminAuth:AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["usertype"] != null && httpContext.Session["usertype"].Equals("Admin"))
            {
                return true;
            }
            return false;
        }
    }
}