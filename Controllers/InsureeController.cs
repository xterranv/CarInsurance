using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    { 
        private CarInsuranceEntities db = new CarInsuranceEntities();
        

        // GET: Insuree
        public ActionResult Admin()
        {
            return View(db.Insurees.ToList());
        }


        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {   
            return View();
        }             

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,/*Quote*/")] Insuree userEntry)
        {
            if (ModelState.IsValid)
            {
                Insuree insuree = new Insuree();                
                insuree.FirstName = userEntry.FirstName;
                insuree.LastName = userEntry.LastName;
                insuree.CarYear = userEntry.CarYear;
                insuree.CarMake = userEntry.CarMake;
                insuree.CarModel = userEntry.CarModel;
                insuree.SpeedingTickets = userEntry.SpeedingTickets;
                insuree.DUI = userEntry.DUI;
                insuree.CoverageType = userEntry.CoverageType;
                insuree.DateOfBirth = userEntry.DateOfBirth;
                insuree.EmailAddress = userEntry.EmailAddress;
                insuree.Quote = userEntry.Quote;
                
                //CALCULATE AGE
                DateTime dob = userEntry.DateOfBirth;

                var today = DateTime.Today;
                int age = today.Year - userEntry.DateOfBirth.Year;

                //CALC QUOTE
                decimal baseQuote = 50M;

                //AGE CHARGE 
                if (age <= 18)
                {
                    insuree.Quote = baseQuote + 100;
                }
                else if (age >= 19 && age <= 25)
                {
                    insuree.Quote = baseQuote + 50;
                }
                else insuree.Quote = baseQuote + 25;

                //CAR YEAR CHARGE
                if (userEntry.CarYear < 2000)
                {
                    insuree.Quote = insuree.Quote + 25;
                }
                else insuree.Quote = insuree.Quote + 25;

                //CAR MAKE/MODEL CHARGE
                if (userEntry.CarMake == "Porsche")
                {
                    insuree.Quote = insuree.Quote + 25;
                }

                if (userEntry.CarMake == "Porsche" && (userEntry.CarModel == "911 Carrera" || userEntry.CarModel == "Carrera"))
                {
                    insuree.Quote = insuree.Quote + 25;
                }

                //SPEEDING TICKET FEE
                int ticketPenalty;

                if (userEntry.SpeedingTickets > 0)
                {
                    ticketPenalty = userEntry.SpeedingTickets * 10;
                    insuree.Quote = insuree.Quote + ticketPenalty;
                }

                //DUI PENALTY FEE
                decimal duiPenalty;

                if (userEntry.DUI == true)
                {
                    duiPenalty = (insuree.Quote / 100 * 25);
                    insuree.Quote = insuree.Quote + duiPenalty;
                }

                //FULL COVERAGE CHARGE
                decimal fullCoverCharge;

                if (userEntry.CoverageType == true)
                {
                    fullCoverCharge = (insuree.Quote / 100 * 50);
                    insuree.Quote = insuree.Quote + fullCoverCharge;
                }

                ViewBag.FinalQuote = insuree.Quote;

                db.Insurees.Add(insuree);
                db.SaveChanges();                
                return View("GetQuote");                
            }

            return View(userEntry);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }


        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Admin");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Admin");
        }
                
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult CoverageAndPolicies()
        {
            return View();
        }

        public ActionResult Discounts()
        {
            return View();
        }
       
    }
}
