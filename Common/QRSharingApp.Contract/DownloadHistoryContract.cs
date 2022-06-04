﻿namespace QRSharingApp.Contract
{
    public class DownloadHistoryContract
    {
        public int Id { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string Date { get; set; }
    }
}