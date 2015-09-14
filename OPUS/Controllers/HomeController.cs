using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace OPUS.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            context.Session["Site"] = "OPUS";
            context.Session["playCode"] = "O";
            context.Session["URL"] = "Not Set";
            string uri = context.Request.Url.ToString();
            context.Session["URL"] = uri;
            ViewBag.URL = uri;
            if (uri.Contains("stcscramble"))
            {
                context.Session["Site"] = "Scramble";
                context.Session["playCode"] = "S";
                context.Session["Group"] = "";
            }
            //else {
            if (User.Identity.Name != "")
            {
                if (User.IsInRole("Admin"))
                {
                    if (Session["Group"] == null && Session["PlayCode"].ToString() != "S")
                    {
                        Session["Group"] = "F";
                    }
                }
                else if (User.IsInRole("Ladies Monitor"))
                {
                    Session["Group"] = "F";
                }
                else if (User.IsInRole("Mens Monitor"))
                {
                    Session["Group"] = "M";
                }
                else
                {
                    if (Session["PlayCode"].ToString() == "S")
                    {
                        Session["Group"] = "";
                    }
                    else
                    {
                        Session["Group"] = "F";
                    }
                }
            }
            //}
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult Help()
        {
            return View();
        }

        [Authorize]
        public ActionResult PrintHelp()
        {
            return View();
        }

        [Authorize]
        public ActionResult CourtAssignmentSheet()
        {
            return View();
        }
    }
}