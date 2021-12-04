using System.Collections.Generic;

namespace FST.Infrastructure.Services.Interfaces
{
    public interface IExcelExportService
    {
        void Export(string pathToSave, IEnumerable<string> columnTitles, IEnumerable<string[]> dataRows);
    }
}