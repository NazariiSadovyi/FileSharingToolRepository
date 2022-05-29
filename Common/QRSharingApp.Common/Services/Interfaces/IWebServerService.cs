using System;
using System.Collections.Generic;

namespace QRSharingApp.Common.Services.Interfaces
{
    public interface IWebServerService
    {
        string WebLocalhostUrl { get; }
        string WebUrl { get; }

        event EventHandler<bool> NetworkChanged;

        string GetFilePath(string fileId);
        IEnumerable<string> GetFilesPathes(string[] fileIds);
    }
}
