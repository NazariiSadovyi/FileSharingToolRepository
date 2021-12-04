using FST.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FST.ViewModel.Interfaces
{
    public interface IDataExportService
    {
        Task Export(IEnumerable<DownloadHistoryModel> importFileStatuses);
    }
}