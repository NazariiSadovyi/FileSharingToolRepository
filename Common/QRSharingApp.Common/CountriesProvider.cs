using Maddalena;
using QRSharingApp.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace QRSharingApp.Common
{
    public static class CountriesProvider
    {
        private static CountryModel[] _countriesPrivate;

        public static CountryModel[] GetAll()
        {
            if (_countriesPrivate != null)
            {
                return _countriesPrivate;
            }

            _countriesPrivate = Country
                .All
                .SelectMany(SelectManyCountries)
                .OrderBy(_ => _.DisplayName)
                .ToArray();

            return _countriesPrivate;
        }

        private static IEnumerable<CountryModel> SelectManyCountries(Country country)
        {
            if (country.CallingCodes == null || country.CallingCodes.Count() == 0)
            {
                yield break;
            }

            foreach (var callingCode in country.CallingCodes.Take(1))
            {
                yield return new CountryModel()
                {
                    Code = country.CountryCode.ToString(),
                    Name = country.CommonName,
                    CallingCode = callingCode
                };
            }
        }
    }
}
