using QRSharingApp.Contract;
using QRSharingApp.DataAccess.Entities;
using System.Collections.Generic;
using System.Linq;

namespace QRSharingApp.WebApplication.Converters
{
    public static class HotFolderConverter
    {
        public static List<HotFolderContract> ToContracts(this IEnumerable<HotFolder> localFiles)
        {
            return localFiles.Select(ToContract).ToList();
        }

        public static HotFolderContract ToContract(this HotFolder entity)
        {
            if (entity == null)
                return null;

            return new HotFolderContract()
            {
                Id = entity.Id,
                FolderPath = entity.FolderPath
            };
        }
    }
}
