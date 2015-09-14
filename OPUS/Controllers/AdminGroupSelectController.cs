using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OPUS.Controllers
{
    public class AdminGroupSelectController : Controller
    {
        // GET: AdminGroupSelect
        public ActionResult Index()
        {
            ViewBag.Group = Session["Group"].ToString();
            return View();
        }

        [HttpPost]
        public ActionResult Index(string Gender)
        {
            Session["Group"] = Gender;

            //Reset Session Data
            Session["courtDates"] = null;
            Session["Players"] = null;
            Session["playedDates"] = null;
            Session["playedDate"] = null;
            Session["Seasons"] = null;
            Session["PastSeason"] = null;
            Session["PastCourtDates"] = null;
            Session["PastCourtDate"] = null;


            return Redirect("/home/Index");
        }
    }
}