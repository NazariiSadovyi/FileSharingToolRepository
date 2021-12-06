using System;
using System.Collections.Generic;

namespace FST.Common.Services.Interfaces
{
    public interface IWebServerService
    {
        string WebLocalhostUrl { get; }

        event EventHandler<bool> NetworkChanged;

        string GetFilePath(string fileId);
        IEnumerable<string> GetFilesPathes(string[] fileIds);
    }
}
