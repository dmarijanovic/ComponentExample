using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Enterwell.Models
{
    public class InvoiceItem
    {
        public int ID { get; set; }

        public String Description { get; set; }

        public int Amount { get; set; }

        public decimal Price { get; set; }

        public virtual Invoice Invoice { get; set; }

        [DisplayName("Total Sum")]
        public decimal TotalSum {
            get
            {
                return Amount * Price;
            }
        }
    }
}