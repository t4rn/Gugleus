using Gugleus.Core.Domain;
using System.Collections.Generic;
using System.IO;

namespace Gugleus.WebUI.Repositories
{
    public interface IFileLogsService
    {
        List<FileInfo> GetAll(EnvType env);
        FileInfo GetFileByName(EnvType value, string fileName);
    }
}
