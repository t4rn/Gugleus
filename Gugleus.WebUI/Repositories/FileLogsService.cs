using Gugleus.Core.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gugleus.WebUI.Repositories
{
    public class FileLogsService : IFileLogsService
    {
        public List<FileInfo> GetAll(EnvType env)
        {
            List<FileInfo> files = new List<FileInfo>();

            string logsPath = PrepareLogsPath(env);

            var dir = new DirectoryInfo(logsPath);
            if (dir.Exists)
            {
                files = dir.GetFiles("*", SearchOption.AllDirectories).ToList();
            }

            return files;
        }

        public FileInfo GetFileByName(EnvType env, string fileName)
        {
            List<FileInfo> allFiles = GetAll(env);

            FileInfo fileInfo = allFiles?.FirstOrDefault(x => x.Name == fileName);

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
                case EnvType.Erexus:
                    path = @"C:\logs\erexus";
                    break;
            }

            return path;
        }
    }
}
