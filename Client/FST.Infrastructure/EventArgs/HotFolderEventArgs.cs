using FST.Infrastructure.Enums;
using FST.Infrastructure.Models;

namespace FST.Infrastructure.EventArgs
{
    public class HotFolderEventArgs
    {
        public HotFolder Folder { get; set; }
        public HotFolderUpdateKind UpdateKind { get; set; }
    }
}