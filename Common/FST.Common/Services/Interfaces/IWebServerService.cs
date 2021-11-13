using System.Collections.Generic;

namespace FST.Common.Services.Interfaces
{
    public interface IWebServerService
    {
        string GetFilePath(string fileId);
        IEnumerable<string> GetFilesPathes(string[] fileIds);
    }
}
