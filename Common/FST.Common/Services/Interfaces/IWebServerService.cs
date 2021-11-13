using System;
using System.Collections.Generic;

namespace FST.Common.Services.Interfaces
{
    public interface IWebServerService
    {
        event EventHandler<bool> NetworkChanged;

        string GetFilePath(string fileId);
        IEnumerable<string> GetFilesPathes(string[] fileIds);
    }
}
