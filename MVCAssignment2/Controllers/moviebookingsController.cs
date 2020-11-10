using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCAssignment2.Models;

namespace MVCAssignment2.Controllers
{
    public class moviebookingsController : Controller
    {
        private moviedbEntities db = new moviedbEntities();

        // GET: moviebookings
        public ActionResult Index()
        {
            var moviebookings = db.moviebookings.Include(m => m.customer).Include(m => m.Screen);
            return View(moviebookings.ToList());
        }

        // GET: moviebookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            moviebooking moviebooking = db.moviebookings.Find(id);
            if (moviebooking == null)
            {
                return HttpNotFound();
            }
            return View(moviebooking);
        }

        // GET: moviebookings/Create
        public ActionResult Create()
        {
            ViewBag.cid = new SelectList(db.customers, "cid", "cname");
            ViewBag.sid = new SelectList(db.Screens, "sid", "sid");
            return View();
        }

        // POST: moviebookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bookingid,sid,cid,noofseats,totalamount")] moviebooking moviebooking)
        {
            Screen st = new Screen();
            var amount = (from s in db.Screens
                          where s.sid == moviebooking.sid
                          select s.Amount).First();
            var totalAmount = amount * moviebooking.noofseats;
            ViewBag.TotalAmount = totalAmount;
            if (ModelState.IsValid)
            {
                moviebooking.totalamount = totalAmount;
                db.moviebookings.Add(moviebooking);
                Screen s2 = db.Screens.Find(st.sid = Convert.ToInt32(moviebooking.sid));
                s2.NoOfSeats = (s2.NoOfSeats - moviebooking.noofseats);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cid = new SelectList(db.customers, "cid", "cname", moviebooking.cid);
            ViewBag.sid = new SelectList(db.Screens, "sid", "sid", moviebooking.sid);
            return View(moviebooking);
        }

        // GET: moviebookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            moviebooking moviebooking = db.moviebookings.Find(id);
            if (moviebooking == null)
            {
                return HttpNotFound();
            }
            ViewBag.cid = new SelectList(db.customers, "cid", "cname", moviebooking.cid);
            ViewBag.sid = new SelectList(db.Screens, "sid", "sid", moviebooking.sid);
            return View(moviebooking);
        }

        // POST: moviebookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bookingid,sid,cid,noofseats,totalamount")] moviebooking moviebooking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(moviebooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cid = new SelectList(db.customers, "cid", "cname", moviebooking.cid);
            ViewBag.sid = new SelectList(db.Screens, "sid", "sid", moviebooking.sid);
            return View(moviebooking);
        }

        // GET: moviebookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            moviebooking moviebooking = db.moviebookings.Find(id);
            if (moviebooking == null)
            {
                return HttpNotFound();
            }
            return View(moviebooking);
        }

        // POST: moviebookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //moviebooking moviebooking = db.moviebookings.Find(id);
            //db.moviebookings.Remove(moviebooking);
            //db.SaveChanges();
            //return RedirectToAction("Index");
            Screen st = new Screen();
            moviebooking moviebooking = db.moviebookings.Find(id);
            
            Screen s2 = db.Screens.Find(st.sid = Convert.ToInt32(moviebooking.sid));
            s2.NoOfSeats = (s2.NoOfSeats + Convert.ToInt32(moviebooking.noofseats));
            db.moviebookings.Remove(moviebooking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
