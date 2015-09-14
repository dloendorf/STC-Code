using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using OPUS.DAL;
using OPUS.Models;

namespace OPUS.Controllers
{
    public class SettingsController : Controller
    {
        private OpusContext db = new OpusContext();

        [Authorize]
        public ActionResult Index()
        {
            var settingData = from a in db.OpusSettings
                                 select a;
            return View(settingData.ToList());
        }

        // GET: OpusSettings/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Settings opusSettings = db.OpusSettings.Find(id);
            if (opusSettings == null)
            {
                return HttpNotFound();
            }
            return View(opusSettings);
        }

        // POST: OpusPlayers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ID,STCEmailAddress,OpusCoordinatorEmailAddress")] Settings opusSettings)
        {
            if (ModelState.IsValid)
            {
                db.Entry(opusSettings).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(opusSettings);
        }
    }
}