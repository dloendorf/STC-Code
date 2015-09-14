using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web;

namespace OPUS
{
    public class STCStartUp
    {
        public static void Init()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            context.Session["Site"] = "OPUS";
            context.Session["playCode"] = "O";
            context.Session["URL"] = "Not Set";
            string uri = context.Request.Url.ToString();
            context.Session["URL"] = uri;
            if (uri.Contains("stcscramble"))
            {
                context.Session["Site"] = "Scramble";
                context.Session["playCode"] = "S";
                context.Session["Group"] = "";
            }
        }
    }
}