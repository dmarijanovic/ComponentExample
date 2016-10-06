using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Enterwell.Models
{
    public class Invoice
    {
        [DisplayName("Invoice Number")]
        public int ID { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Created { get; set; }

        public DateTime Due { get; set; }

        [DisplayName("Customer Name")]
        [Required]
        public String CustomerName { get; set; }

        [DisplayName("Vat")]
        public string VatModuleIdentifier { get; set; }

        [DisplayName("Created By")]
        public virtual ApplicationUser CreatedBy { get; set; }

        public virtual ICollection<InvoiceItem> Items { get; set; }

        [DisplayName("Total Sum")]
        public decimal TotalSum {
            get
            {
                return Items != null ? Items.Sum(i => i.TotalSum) : 0;
            }
         }

        [DisplayName("Total Sum With Vat")]
        public decimal TotalSumWithVat
        {
            get
            {
                return ModuleManager.Instance.countrySetting.CalculateVat(TotalSum, VatModuleIdentifier);
            }
        }

        public string CurrencySimbol
        {
            get
            {
                return ModuleManager.Instance.countrySetting.CurrencySymbol(VatModuleIdentifier);
            }
        }

        public static int PendingInvoiceCount(ApplicationDbContext db)
        {
            var invoices = from i in db.Invoices
                           where i.Due > DateTime.Now
                           select i;

            return invoices.Count();
        }

    }
}