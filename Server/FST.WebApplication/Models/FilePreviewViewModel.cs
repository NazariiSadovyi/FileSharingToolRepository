using System;

namespace FST.WebApplication.Models
{
    public class FilePreviewViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public bool IsPhoto { get; set; }
        public bool IsVideo { get; set; }
        public string QRCodeAdress { get; set; }
        public string ThumbnailAdress { get; set; }
    }
}
