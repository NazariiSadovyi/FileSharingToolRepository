using System.Collections.Generic;

namespace QRSharingApp.WebApplication.Models
{
    public class FilesGalleryViewModel
    {
        public List<FilePreviewViewModel> FilePreviews { get; set; }
        public string GalleryUrlQRImageData { get; set; }
        public string WifiQRImageData { get; set; }

        public FilesGalleryViewModel()
        {
            FilePreviews = new List<FilePreviewViewModel>();
        }
    }
}
