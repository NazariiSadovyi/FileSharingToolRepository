using QRSharingApp.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QRSharingApp.ViewModel.Interfaces
{
    public interface IDataExportService
    {
        Task Export(IEnumerable<DownloadHistoryModel> importFileStatuses);
    }
}