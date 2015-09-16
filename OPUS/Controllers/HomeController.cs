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
            var request = ControllerContext.RequestContext.HttpContext.Request;
            //System.Web.HttpContext context = System.Web.HttpContext.Current;
            Session["Site"] = "Scramble";
            Session["playCode"] = "S";
            Session["Group"] = "";
            Session["URL"] = "Not Set";
            string uri = request.Url.ToString();
            Session["URL"] = uri;
            ViewBag.URL = uri;
            if (uri.Contains("stcscramble"))
            {
                Session["Site"] = "Scramble";
                Session["playCode"] = "S";
                Session["Group"] = "";
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