using QRSharingApp.Infrastructure.Enums;
using QRSharingApp.Infrastructure.Models;

namespace QRSharingApp.Infrastructure.EventArgs
{
    public class HotFolderEventArgs
    {
        public HotFolder Folder { get; set; }
        public HotFolderUpdateKind UpdateKind { get; set; }
    }
}