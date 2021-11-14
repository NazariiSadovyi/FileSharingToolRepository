using Dropbox.Api;
using NPOI.XSSF.UserModel;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FST.Activation
{
    public class ActivationViaDropbox : IActivationProvider
    {
        private static readonly LicenseKeyProvider _licenseKeyProvider = new LicenseKeyProvider();
        private readonly DropboxClient _dropboxClient;
        private const string _dropboxAccessToken = "TyJLi8pZPqQAAAAAAAAAAV2Cm99SFQcp8zb56rJW8qniXCrYS78gWWG4KgUxcq3y";

        public ActivationViaDropbox()
        {
            _dropboxClient = new DropboxClient(_dropboxAccessToken);
        }

        public async Task<bool> CheckAndSaveLicense(string key)
        {
            var isValid = await CheckDropboxTableContainsKeyAsync(key);
            if (!isValid)
            {
                return false;
            }

            await LicenceUpdate(key);

            return true;
        }

        public string GetSavedLicenseKey()
        {
            return _licenseKeyProvider.Key;
        }

        public async Task<bool> IsActivatedCheck()
        {
            var currentKey = _licenseKeyProvider.Key;
            if (string.IsNullOrEmpty(currentKey))
            {
                return false;
            }

            return await CheckDropboxTableContainsKeyAsync(currentKey);
        }

        public async Task LicenceUpdate(string key)
        {
            await Task.Run(() => _licenseKeyProvider.Key = key);
        }

        public async Task DeactivateLicense()
        {
            await Task.Run(() => _licenseKeyProvider.Key = string.Empty);
        }

        private async Task<bool> CheckDropboxTableContainsKeyAsync(string key)
        {
            var result = await _dropboxClient.Files.DownloadAsync("/FST_Authentification_keys.xlsx");
            var contentStream = await result.GetContentAsStreamAsync();
            var sha256Key = GetKeyAsSHA256(key);
            var workbook = new XSSFWorkbook(contentStream);
            var firstSheet = workbook.GetSheetAt(0);
            for (int row = 0; row <= firstSheet.LastRowNum; row++)
            {
                if (firstSheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    var cellStringValue = firstSheet.GetRow(row).GetCell(0).StringCellValue;
                    if (cellStringValue == sha256Key)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private string GetKeyAsSHA256(string key)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(key));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
