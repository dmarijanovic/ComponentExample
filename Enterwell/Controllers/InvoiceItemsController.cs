using System;
using System.Net;
using System.Web.Mvc;
using Enterwell.Models;

namespace Enterwell.Controllers
{
    [Authorize]
    public class InvoiceItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: InvoiceItems/Create
        public ActionResult Create(int invoiceId)
        {
            Invoice invoice = db.Invoices.Find(invoiceId);

            if(invoice == null)
            {
                return HttpNotFound();
            }

            ViewBag.InvoiceId = invoiceId;
            ViewBag.CurrencySimbol = invoice.CurrencySimbol;
            return View();
        }

        // POST: InvoiceItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Description,Amount,Price,InvoiceId")] CreateInvoiceItemViewModel createInvoiceItemViewModel)
        {
            InvoiceItem invoiceItem = null;
            if (ModelState.IsValid)
            {
                invoiceItem = new InvoiceItem
                {
                    Amount = createInvoiceItemViewModel.Amount,
                    Description = createInvoiceItemViewModel.Description,
                    Price = createInvoiceItemViewModel.Price,
                    Invoice = db.Invoices.Find(createInvoiceItemViewModel.InvoiceId)
                };

                if (invoiceItem.Invoice != null)
                {
                    db.InvoiceItems.Add(invoiceItem);
                    db.SaveChanges();
                    return RedirectToAction("Edit", "Invoices", new { invoiceItem.Invoice.ID });
                }
                else
                {
                    return HttpNotFound("Invoice ID not found.");
                }
            }

            return View(invoiceItem);
        }

        // GET: InvoiceItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvoiceItem invoiceItem = db.InvoiceItems.Find(id);
            if (invoiceItem == null)
            {
                return HttpNotFound();
            }
            return View(invoiceItem);
        }

        // POST: InvoiceItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InvoiceItem invoiceItem = db.InvoiceItems.Find(id);
            int invoiceId = invoiceItem.Invoice.ID;
            db.InvoiceItems.Remove(invoiceItem);
            db.SaveChanges();
            return RedirectToAction("Edit", "Invoices", new { id = invoiceId });
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
