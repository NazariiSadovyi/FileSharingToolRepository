using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using QRSharingApp.Common.Settings.Interfaces;
using System.Threading.Tasks;

namespace QRSharingApp.WebApplication
{
    public class RequestCultureProvider : IRequestCultureProvider
    {
        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var webSetting = httpContext.RequestServices.GetService<IWebSetting>();
            var cultureCode = webSetting.WebCultureCode ?? "en-US";
            var providerCultureResult = new ProviderCultureResult(cultureCode, cultureCode);

            return Task.FromResult(providerCultureResult);
        }
    }
}
