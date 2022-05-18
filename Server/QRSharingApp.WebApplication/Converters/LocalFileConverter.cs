using QRSharingApp.Contract.LocalFile;
using QRSharingApp.DataAccess.Entities;
using System.Collections.Generic;
using System.Linq;

namespace QRSharingApp.WebApplication.Converters
{
    public static class LocalFileConverter
    {
        public static List<LocalFileContract> ToContracts(this IEnumerable<LocalFile> localFiles)
        {
            return localFiles.Select(ToContract).ToList();
        }

        public static LocalFileContract ToContract(this LocalFile localFile)
        {
            if (localFile == null)
                return null;

            return new LocalFileContract()
            {
                Id = localFile.Id,
                Name = localFile.Name,
                Path = localFile.Path
            };
        }
    }
}
