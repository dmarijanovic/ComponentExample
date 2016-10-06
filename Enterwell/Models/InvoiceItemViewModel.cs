using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Enterwell.Models
{
    public class CreateInvoiceItemViewModel
    {
        public String Description { get; set; }

        public int Amount { get; set; }

        public decimal Price { get; set; }

        public int InvoiceId { get; set; }
    }
}