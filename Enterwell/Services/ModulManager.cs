using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Web.Mvc;
using System.Globalization;
using System.IO;

namespace Enterwell
{

    public interface IVat
    {
        int Vat { get; }
    }

    public interface ICountryCode
    {
        String CountryCode { get; }

        String Rate { get; }
    }

    public interface ICountrySetting
    {
        decimal CalculateVat(decimal value, string countryCode);

        List<SelectListItem> VatOptions();

        string CurrencySymbol(string moduleIdentifier);

        string DisplayName(string moduleIdentifier);
    }

    [Export(typeof(ICountrySetting))]
    public class CountrySetting : ICountrySetting
    {

        [ImportMany(typeof(IVat))]
        public IEnumerable<Lazy<IVat, ICountryCode>> countrysVat;


        public decimal CalculateVat(decimal value, string vatModule)
        {
            foreach (Lazy<IVat, ICountryCode> i in countrysVat)
            {
                string moduleIdentifier = GetModuleIdentifier(i);

                if (moduleIdentifier.Equals(vatModule))
                {
                    return value + (value * (i.Value.Vat / 100m));
                }
            };

            return 0;
        }

        public List<SelectListItem> VatOptions()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (Lazy<IVat, ICountryCode> i in countrysVat)
            {
                string moduleIdentifier = GetModuleIdentifier(i);
                string displayName = String.Concat(i.Metadata.CountryCode, " (", i.Value.Vat, "%)");

                list.Add(new SelectListItem { Text = displayName, Value = moduleIdentifier });
            };

            return list;
        }

        public string DisplayName(string moduleIdentifier)
        {
            Lazy<IVat, ICountryCode> module = countrysVat.First(m => GetModuleIdentifier(m).Equals(moduleIdentifier));

            return String.Concat(module.Metadata.CountryCode, " (", module.Value.Vat, "%)");
        }

        public string CurrencySymbol(string moduleIdentifier)
        {
            string countryCode;
            if (!GetCountryCode(moduleIdentifier, out countryCode))
            {
                return "";
            }

            string symbol = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Select(culture =>
                {
                    try
                    {
                        return new RegionInfo(culture.LCID);
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(ri => ri != null && ri.TwoLetterISORegionName == countryCode)
                .Select(ri => ri.CurrencySymbol)
                .FirstOrDefault();

            return symbol;
        }

        public string GetModuleIdentifier(Lazy<IVat, ICountryCode> i)
        {
            return String.Concat(i.Metadata.CountryCode, "-", i.Metadata.Rate);
        }

        private bool GetCountryCode(string moduleIdentifier, out string countryCode)
        {
            countryCode = null;
            string[] identifierParts = moduleIdentifier.Split('-');
            if (identifierParts.Length != 0)
            {
                countryCode = identifierParts[0];
            }

            return countryCode != null;
        }

    }

    public class ModuleManager
    {
        private CompositionContainer _container;

        [Import(typeof(ICountrySetting))]
        public ICountrySetting countrySetting;

        private static ModuleManager instance;

        private ModuleManager()
        {
            Init();
        }

        private void Init()
        {
            var catalog = new AggregateCatalog();
            string extansionsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extansions");

            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            catalog.Catalogs.Add(new DirectoryCatalog(extansionsPath));

            _container = new CompositionContainer(catalog);

            try
            {
                this._container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public static ModuleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ModuleManager();
                }
                return instance;
            }
        }

    }
}