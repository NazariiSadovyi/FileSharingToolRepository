using QRSharingApp.Infrastructure.Services.Interfaces;
using System;
using System.IO;
using System.Windows.Forms;

namespace QRSharingApp.Infrastructure.Services
{
    public class FileExplorerService : IFileExplorerService
    {
        private readonly string _downloadDirectory;

        public FileExplorerService()
        {
            var userProfileDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _downloadDirectory = Path.Combine(userProfileDirectory, "Downloads");
        }

        public string SelectFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                return fbd.SelectedPath;
            }
        }

        public string SelectFile(string filter)
        {
            var openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = filter,
                FilterIndex = 0,
                RestoreDirectory = true
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }

            return string.Empty;
        }

        public string SaveFile(string defaultFileName, string filter, string dialogTitle = "Save file")
        {
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = _downloadDirectory,
                FileName = defaultFileName,
                RestoreDirectory = true,
                Title = dialogTitle,
                Filter = filter
            };
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }

            return string.Empty;
        }
    }
}
