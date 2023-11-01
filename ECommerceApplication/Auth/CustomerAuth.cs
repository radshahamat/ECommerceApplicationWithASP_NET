using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceApplication.Auth
{
    public class CustomerAuth : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["usertype"] != null && httpContext.Session["usertype"].Equals("Customer"))
            {
                return true;
            }
            return false;
        }
    }
}