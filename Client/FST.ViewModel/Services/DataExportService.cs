using FST.Infrastructure.Models;
using FST.Infrastructure.Services.Interfaces;
using FST.ViewModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace FST.ViewModel.Services
{
    public class DataExportService : IDataExportService
    {
        private const string DownloadHistoryFileName = "Download history";

        [Dependency]
        public IExcelExportService ExcelExportService { get; set; }
        [Dependency]
        public IFileExplorerService FileExplorerService { get; set; }
        [Dependency]
        public IApplicationTaskUtility ApplicationTaskUtility { get; set; }

        public async Task Export(IEnumerable<DownloadHistoryModel> importFileStatuses)
        {
            await Export(
                DownloadHistoryFileName,
                new string[] { "File Id", "File Name", "File Path", "User Name", "User Email", "User Phone", "Download Date" },
                importFileStatuses.Select(_ => new[] {
                    _.FileId,
                    _.FileName,
                    _.FilePath,
                    _.UserName,
                    _.UserEmail,
                    _.UserPhone,
                    _.Date.ToLongDateString()
                })
            );

        }

        private async Task Export(string fileName, string[] titles, IEnumerable<string[]> data)
        {
            var exportFileName = $"{fileName} {DateTime.Now:MM.dd.yyyy}";
            var exportFilePath = string.Empty;
            Application.Current.Dispatcher.Invoke(() =>
            {
                exportFilePath = FileExplorerService.SaveFile(exportFileName, "Excel(*.xlsx)|*.xlsx");
            });
            if (string.IsNullOrEmpty(exportFilePath))
            {
                return;
            }

            await ApplicationTaskUtility.ExecuteFetchDataAsync(() =>
            {
                return Task.Run(() => ExcelExportService.Export(exportFilePath, titles, data));
            }, "Exporting data...");
        }
    }
}
