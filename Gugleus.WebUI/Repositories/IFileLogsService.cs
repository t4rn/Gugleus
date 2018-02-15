using Gugleus.Core.Domain;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Gugleus.WebUI.Repositories
{
    public interface IFileLogsService
    {
        List<FileInfo> GetAllAsync(EnvType env);
        FileInfo GetFileByNameAsync(EnvType value, string fileName);
    }
}
