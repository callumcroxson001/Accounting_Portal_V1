using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using sqlupdate.Models;
using System.Reflection.Emit;
using System.Web.UI.WebControls;
using System.Web.UI;
using Rotativa;
using System.IO;

namespace sqlupdate.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //Confirmation view
        public ActionResult Confirmation()
        {
            return View();
        }


        //Form functions
        [HttpPost]
        public ActionResult Cash(DateTime date, decimal amount)
        {
            testEntities3 db = new testEntities3();
            Donation temp = new Donation();
            temp.amount = amount;
            temp.type = "C";
            temp.date = date;
            db.Donations.Add(temp);
            db.SaveChanges();
            return RedirectToAction("Confirmation", "Home");
        }

        [HttpPost]
        public ActionResult en(int did, string name, string purpose, string specifics, decimal amount, DateTime date)
        {
            testEntities3 db = new testEntities3();
            Donation temp = new Donation();
            temp.date = date;
            temp.amount = amount;
            temp.DID = did;
            temp.type = "E";
            temp.specifics = specifics;

            List<Purpos> p = db.Purposes.Where(x => x.name.Contains(purpose)).ToList();
            int pid = p.First().ID;

            temp.purpose = pid;

            db.Donations.Add(temp);
            db.SaveChanges();

            ViewBag.ID = temp.DID;
            ViewBag.Name = name;
            return View();
        }
        
        [HttpPost]
        public ActionResult Online(int did, string name, string purpose, string specifics, decimal amount, DateTime date)
        {
            testEntities3 db = new testEntities3();
            Donation temp = new Donation();
            temp.date = date;
            temp.amount = amount;
            temp.DID = did;
            temp.type = "O";
            temp.specifics = specifics;

            List<Purpos> p = db.Purposes.Where(x => x.name.Contains(purpose)).ToList();
            int pid = p.First().ID;

            temp.purpose = pid;

            db.Donations.Add(temp);
            db.SaveChanges();

            ViewBag.ID = temp.DID;
            ViewBag.Name = name;
            return View();
        }
        
        [HttpPost]
        public ActionResult Expenditure(DateTime date, string subcategory, decimal amount, string invoiceRef, string notes)
        {
            testEntities3 db = new testEntities3();
            Expenditure temp = new Expenditure();
            temp.date = date;
            temp.amount = amount;
            temp.invoiceReference = invoiceRef;
            temp.notes = notes;

            List<SubCategory> p = db.SubCategories.Where(x => x.name.Contains(subcategory)).ToList();
            int scid = p.First().ID;

            temp.SCID = scid;

            db.Expenditures.Add(temp);
            db.SaveChanges();

            return RedirectToAction("Confirmation", "Home");
        }

        [HttpPost]
        public ActionResult Donor(ViewModel model)
        {
            Donor temp = new Donor();
            temp.name = model.donor.name;
            temp.email = model.donor.email;
            temp.address = model.donor.address;
            temp.homePhone = model.donor.homePhone;
            temp.mobilePhone = model.donor.mobilePhone;
            temp.dateOfBirth = model.donor.dateOfBirth;
            testEntities3 db = new testEntities3();
            db.Donors.Add(temp);
            db.SaveChanges();

            return RedirectToAction("Confirmation", "Home");
        }

        //Search functions for envelope and online forms
        [HttpPost]
        public ActionResult SearchDonorName(Donor donor)
        {
            Donor temp = new Donor();
            temp.ID = donor.ID;
            temp.name = donor.name;
            testEntities3 db = new testEntities3();
            List<Donor> d = db.Donors.Where(x => (x.name.Contains(temp.name)) || (x.ID == temp.ID)).ToList();

            return View(d);
        }
        
        [HttpPost]
        public ActionResult SearchDonorNameOnline(Donor donor)
        {
            Donor temp = new Donor();
            temp.ID = donor.ID;
            temp.name = donor.name;
            testEntities3 db = new testEntities3();
            List<Donor> d = db.Donors.Where(x => (x.name.Contains(temp.name)) || (x.ID == temp.ID)).ToList();

            return View(d);
        }


        //Reports

        //Monthly Donations
        public ActionResult Monthly(string month, string year)
        {
            testEntities3 db = new testEntities3();
            int donMonth = Int32.Parse(month);
            int donYear = Int32.Parse(year);
            List<Donation> don = db.Donations.Where(x => x.date.Month.Equals(donMonth) && x.date.Year.Equals(donYear)).OrderBy(x => x.date).ToList();

            decimal total = don.Sum(x => x.amount);
            decimal cTotal = don.Where(x => x.type.Contains("C")).Sum(x => x.amount);
            decimal eTotal = don.Where(x => x.type.Contains("E")).Sum(x => x.amount);
            decimal oTotal = don.Where(x => x.type.Contains("O")).Sum(x => x.amount);

            if (donMonth == 1)
            {
                ViewBag.MonthName = "January";
            }
            else if(donMonth == 2)
            {
                ViewBag.MonthName = "February";
            }
            else if(donMonth == 3)
            {
                ViewBag.MonthName = "March";
            }
            else if(donMonth == 4)
            {
                ViewBag.MonthName = "April";
            }
            else if(donMonth == 5)
            {
                ViewBag.MonthName = "May";
            }
            else if(donMonth == 6)
            {
                ViewBag.MonthName = "June";
            }
            else if(donMonth == 7)
            {
                ViewBag.MonthName = "July";
            }
            else if(donMonth == 8)
            {
                ViewBag.MonthName = "August";
            }
            else if(donMonth == 9)
            {
                ViewBag.MonthName = "September";
            }
            else if(donMonth == 10)
            {
                ViewBag.MonthName = "October";
            }
            else if(donMonth == 11)
            {
                ViewBag.MonthName = "November";
            }
            else if(donMonth == 12)
            {
                ViewBag.MonthName = "December";
            }

            ViewBag.Total = total;
            ViewBag.cTotal = cTotal;
            ViewBag.eTotal = eTotal;
            ViewBag.oTotal = oTotal;
            ViewBag.Year = year;
            return View(don);
        }

        //Yearly Donations 
        public ActionResult Yearly(string year)
        {
            testEntities3 db = new testEntities3();
            int donYear = Int32.Parse(year);
            List<Donation> don = db.Donations.Where(x => x.date.Year.Equals(donYear)).OrderBy(x => x.date).ToList();

            decimal total = don.Sum(x => x.amount);
            decimal cTotal = don.Where(x => x.type.Contains("C")).Sum(x => x.amount);
            decimal eTotal = don.Where(x => x.type.Contains("E")).Sum(x => x.amount);
            decimal oTotal = don.Where(x => x.type.Contains("O")).Sum(x => x.amount);

            ViewBag.Total = total;
            ViewBag.cTotal = cTotal;
            ViewBag.eTotal = eTotal;
            ViewBag.oTotal = oTotal;
            ViewBag.Year = year;
            return View(don);
        }

        //Donor tax certificate
        public ActionResult TaxCertificate(Donor donor, string year)
        {
            testEntities3 db = new testEntities3();
            int donYear = Int32.Parse(year);
            Donor temp = new Donor();
            temp.ID = donor.ID;
            temp.name = donor.name;
            List<Donor> d = db.Donors.Where(x => (x.name.Contains(temp.name)) || (x.ID == temp.ID)).ToList();

            int did = d.First().ID;

            List<Donation> donDonations = db.Donations.Where(x => (x.date.Year.Equals(donYear)) && (x.DID == did)).OrderBy(x => x.date).ToList();
            decimal total = donDonations.Sum(x => x.amount);
            decimal donorEnTotal = donDonations.Where(x => x.type.Contains("E")).Sum(x => x.amount);
            decimal donorOnTotal = donDonations.Where(x => x.type.Contains("O")).Sum(x => x.amount);

            ViewBag.Name = d.First().name;
            ViewBag.ID = d.First().ID;
            ViewBag.Total = total;
            ViewBag.enTotal = donorEnTotal;
            ViewBag.onTotal = donorOnTotal;
            return View(donDonations);
        }

        //Monthly expenses report
        public ActionResult ExMonthly(string month, string year)
        {
            testEntities3 db = new testEntities3();
            int donMonth = Int32.Parse(month);
            int donYear = Int32.Parse(year);
            List<Expenditure> don = db.Expenditures.Where(x => x.date.Month.Equals(donMonth) && x.date.Year.Equals(donYear)).OrderBy(x => x.date).ToList();

            decimal total = don.Sum(x => x.amount);

            if (donMonth == 1)
            {
                ViewBag.MonthName = "January";
            }
            else if (donMonth == 2)
            {
                ViewBag.MonthName = "February";
            }
            else if (donMonth == 3)
            {
                ViewBag.MonthName = "March";
            }
            else if (donMonth == 4)
            {
                ViewBag.MonthName = "April";
            }
            else if (donMonth == 5)
            {
                ViewBag.MonthName = "May";
            }
            else if (donMonth == 6)
            {
                ViewBag.MonthName = "June";
            }
            else if (donMonth == 7)
            {
                ViewBag.MonthName = "July";
            }
            else if (donMonth == 8)
            {
                ViewBag.MonthName = "August";
            }
            else if (donMonth == 9)
            {
                ViewBag.MonthName = "September";
            }
            else if (donMonth == 10)
            {
                ViewBag.MonthName = "October";
            }
            else if (donMonth == 11)
            {
                ViewBag.MonthName = "November";
            }
            else if (donMonth == 12)
            {
                ViewBag.MonthName = "December";
            }

            ViewBag.Total = total;
            ViewBag.Year = year;
            return View(don);
        }

        public ActionResult ExYearly(string year)
        {
            testEntities3 db = new testEntities3();
            int donYear = Int32.Parse(year);
            List<Expenditure> don = db.Expenditures.Where(x => x.date.Year.Equals(donYear)).OrderBy(x => x.date).ToList();

            decimal total = don.Sum(x => x.amount);

            ViewBag.Total = total;
            ViewBag.Year = year;
            return View(don);
        }

        public ActionResult PurposeTotal(string purpose)
        {
            testEntities3 db = new testEntities3();
            List<Purpos> p = db.Purposes.Where(x => x.name.Contains(purpose)).ToList();

            int temp = p.First().ID;

            List<Donation> purp = db.Donations.Where(x => x.purpose == temp).ToList();
            decimal total = purp.Sum(x => x.amount);

            ViewBag.Total = total;
            ViewBag.PurposeName = p.First().name.ToUpper();
            return View(purp);
        }

        public ActionResult DonorList()
        {
            testEntities3 db = new testEntities3();
            List<Donor> donors = db.Donors.ToList();

            return View(donors);
        }

        public ActionResult DonorListPdf()
        {
            var report = new ActionAsPdf("DonorList");
            return report;
        }

        public ActionResult ViewList(string viewList)
        {
            testEntities3 db = new testEntities3();
            
            if(viewList == "Purposes")
            {
                List<Purpos> list = db.Purposes.ToList();
                ViewBag.List = "Purposes";
                return View(list);
            }
            if(viewList == "Category")
            {
                List<Category> list = db.Categories.ToList();
                ViewBag.List = "Category";
                return View(list);
            }
            if(viewList == "SubCategory")
            {
                List<SubCategory> list = db.SubCategories.ToList();
                ViewBag.List = "Sub-Category";
                return View(list);
            }

            return View();
        }
    }
}