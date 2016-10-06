using Enterwell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BihCountryModule
{
    public class ModuleExport
    {
        [Export(typeof(IVat))]
        [ExportMetadata("CountryCode", "BA")]
        [ExportMetadata("Rate", "Standard")]
        public class BihVat : IVat
        {
            public int Vat
            {
                get
                {
                    return 17;
                }
            }
        }
    }
}
