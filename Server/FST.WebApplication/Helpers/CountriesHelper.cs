using FST.WebApplication.Models;
using Maddalena;
using System.Collections.Generic;
using System.Linq;

namespace FST.WebApplication.Helpers
{
    public static class CountriesHelper
    {
        private static IEnumerable<CountryViewModel> _countriesPrivate; 

        public static IEnumerable<CountryViewModel> GetAll()
        {
            if (_countriesPrivate != null)
            {
                return _countriesPrivate;
            }

            _countriesPrivate = Country.All.SelectMany(SelectManyCountries);

            return _countriesPrivate;
        }

        private static IEnumerable<CountryViewModel> SelectManyCountries(Country country)
        {
            if (country.CallingCodes == null || country.CallingCodes.Count() == 0)
            {
                yield break;
            }

            foreach (var callingCode in country.CallingCodes.Take(1))
            {
                yield return new CountryViewModel() 
                {
                    Code = country.CountryCode.ToString(),
                    Name = country.CommonName,
                    CallingCode = callingCode
                };
            }
        }
    }
}
