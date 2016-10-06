using Enterwell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CroatiaCountryModule
{
    public class ModuleExport
    {
        [Export(typeof(IVat))]
        [ExportMetadata("CountryCode", "HR")]
        [ExportMetadata("Rate", "Standard")]
        public class CroStandardVat : IVat
        {
            public int Vat
            {
                get
                {
                    return 25;
                }
            }
        }

        [Export(typeof(IVat))]
        [ExportMetadata("CountryCode", "HR")]
        [ExportMetadata("Rate", "Reduced")]
        public class CroReducedVat : IVat
        {
            public int Vat
            {
                get
                {
                    return 13;
                }
            }
        }

        [Export(typeof(IVat))]
        [ExportMetadata("CountryCode", "HR")]
        [ExportMetadata("Rate", "MoreReduced")]
        public class CroMoreReducedVat : IVat
        {
            public int Vat
            {
                get
                {
                    return 5;
                }
            }
        }
    }
}
