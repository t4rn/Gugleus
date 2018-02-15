using Gugleus.Core.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gugleus.WebUI.Repositories
{
    public class FileLogsService : IFileLogsService
    {
        public List<FileInfo> GetAllAsync(EnvType env)
        {
            List<FileInfo> files = new List<FileInfo>();

            string logsPath = PrepareLogsPath(env);

            var dir = new DirectoryInfo(logsPath);
            if (dir.Exists)
            {
                files = dir.GetFiles().ToList();
            }

            return files;
        }

        public FileInfo GetFileByNameAsync(EnvType env, string fileName)
        {
            string logsPath = PrepareLogsPath(env);

            FileInfo fileInfo = new FileInfo($"{logsPath}\\{fileName}");

            return fileInfo;
        }

        private string PrepareLogsPath(EnvType env)
        {
            string path = null;

            switch (env)
            {
                case EnvType.Dev:
                    path = @"C:\logs\dev";
                    break;
                case EnvType.Rc:
                    path = @"C:\logs\rc";
                    break;
                case EnvType.Prod:
                    path = @"C:\logs\prod";
                    break;
            }

            return path;
        }
    }
}
