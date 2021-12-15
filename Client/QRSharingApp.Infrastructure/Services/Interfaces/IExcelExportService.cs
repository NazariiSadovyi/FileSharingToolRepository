using System.Collections.Generic;

namespace QRSharingApp.Infrastructure.Services.Interfaces
{
    public interface IExcelExportService
    {
        void Export(string pathToSave, IEnumerable<string> columnTitles, IEnumerable<string[]> dataRows);
    }
}