using System;

namespace QRSharingApp.WebApplication.Models
{
    public class DownloadHistoryViewModel
    {
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public DateTime Date { get; set; }
    }
}
