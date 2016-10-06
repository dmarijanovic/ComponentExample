using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Enterwell.Models;

namespace Enterwell.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            HomeIndexViewModel model = new HomeIndexViewModel();
            model.TotalInvoiceCount = db.Invoices.Count();
            model.TotalUserCount = db.Users.Count();
            model.TotalPendingInvoiceCount = Invoice.PendingInvoiceCount(db);

            return View(model);
        }
    }
}