using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FST.Infrastructure.Services.Interfaces
{
    public interface IWebServerService
    {
        string GetFilePath(string fileId);
        IEnumerable<string> GetFilesPathes(string[] fileIds);
    }
}
