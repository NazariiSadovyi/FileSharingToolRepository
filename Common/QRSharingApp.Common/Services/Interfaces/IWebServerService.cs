using System;
using System.Collections.Generic;

namespace QRSharingApp.Common.Services.Interfaces
{
    public interface IWebServerService
    {
        string WebLocalhostUrl { get; }

        event EventHandler<bool> NetworkChanged;

        string GetFilePath(string fileId);
        IEnumerable<string> GetFilesPathes(string[] fileIds);
    }
}
