using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Enterwell.Models;
using Microsoft.AspNet.Identity;
using PagedList;

namespace Enterwell.Controllers
{
    [Authorize]
    public class InvoicesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Invoices
        public ActionResult Index(string sortOrder, string filter, int? page)
        {
            ViewBag.Filter = filter;

            var invoices = from i in db.Invoices
                           select i;

            if (!String.IsNullOrEmpty(filter))
            {
                invoices = invoices.Where(s => s.CustomerName.Contains(filter)
                                       || s.CreatedBy.UserName.Contains(filter)
                                       || s.ID.ToString().Contains(filter));
            }

            switch (sortOrder)
            {
                case "id":
                    invoices = invoices.OrderBy(i => i.ID);
                    break;
                case "id_desc":
                    invoices = invoices.OrderByDescending(i => i.ID);
                    break;
                case "due":
                    invoices = invoices.OrderBy(i => i.Due);
                    break;
                case "due_desc":
                    invoices = invoices.OrderByDescending(i => i.Due);
                    break;
                case "customer_name":
                    invoices = invoices.OrderBy(i => i.CustomerName);
                    break;
                case "customer_name_desc":
                    invoices = invoices.OrderByDescending(i => i.CustomerName);
                    break;
                case "created":
                    invoices = invoices.OrderBy(i => i.Created);
                    break;
                case "created_desc":
                default:
                    invoices = invoices.OrderByDescending(i => i.Created);
                    break;
            }

            int pageNumber = (page ?? 1);

            return View(invoices.ToPagedList(pageNumber, 10));
        }

        // GET: Invoices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }

            ViewBag.VatDisplayName = ModuleManager.Instance.countrySetting.DisplayName(invoice.VatModuleIdentifier);
            return View(invoice);
        }

        // GET: Invoices/Create
        public ActionResult Create()
        {
            ViewBag.VatOptions = ModuleManager.Instance.countrySetting.VatOptions();
            Invoice invoice = new Invoice { Due = DateTime.Now.AddMonths(1) };
            
            return View(invoice);
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Created,Due,CustomerName,VatModuleIdentifier")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                string currentUserId = User.Identity.GetUserId();
                ApplicationUser currentUser = db.Users.FirstOrDefault(x => x.Id == currentUserId);
                invoice.CreatedBy = currentUser;
                db.Invoices.Add(invoice);
                db.SaveChanges();
                return RedirectToAction("Edit", "Invoices", new { invoice.ID });
            }

            ViewBag.VatOptions = ModuleManager.Instance.countrySetting.VatOptions();
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }

            ViewBag.VatOptions = ModuleManager.Instance.countrySetting.VatOptions();
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Created,Due,CustomerName,Items,VatModuleIdentifier")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                if (invoice.Items != null)
                {
                    foreach (var i in invoice.Items)
                    {
                        db.Entry(i).State = EntityState.Modified;
                    }
                }
                
                db.Entry(invoice).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            invoice = db.Invoices.Find(invoice.ID);
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invoice invoice = db.Invoices.Find(id);
            if (invoice == null)
            {
                return HttpNotFound();
            }
            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invoice invoice = db.Invoices.Find(id);
            db.Invoices.Remove(invoice);
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
