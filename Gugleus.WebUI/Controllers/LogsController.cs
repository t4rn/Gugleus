using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.WebUI.AutoMapper;
using Gugleus.WebUI.Models.Logs;
using Gugleus.WebUI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using X.PagedList;

namespace Gugleus.WebUI.Controllers
{
    public class LogsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IFileLogsService _fileLogsService;
        private readonly ILogger<LogsController> _logger;

        // TODO: clean sql -> erexus instead of gugleus
        public LogsController(IMapper mapper, IFileLogsService fileLogsService, ILogger<LogsController> logger)
        {
            _mapper = mapper;
            _fileLogsService = fileLogsService;
            _logger = logger;
        }

        [Route("[controller]/Dev/{page?}")]
        public IActionResult Dev(int? page)
        {
            FileLogListVM model = PrepareModel(EnvType.Dev, page);
            return View("FileLogsList", model);
        }

        [Route("[controller]/Rc/{page?}")]
        public IActionResult Rc(int? page)
        {
            FileLogListVM model = PrepareModel(EnvType.Rc, page);
            return View("FileLogsList", model);
        }

        [Route("[controller]/Prod/{page?}")]
        public IActionResult Prod(int? page)
        {
            FileLogListVM model = PrepareModel(EnvType.Prod, page);
            return View("FileLogsList", model);
        }

        [Route("[controller]/Erexus/{page?}")]
        public IActionResult Erexus(int? page)
        {
            FileLogListVM model = PrepareModel(EnvType.Erexus, page);
            return View("FileLogsList", model);
        }

        private FileLogListVM PrepareModel(EnvType env, int? page, int pageSize = 20)
        {
            FileLogListVM model = new FileLogListVM();
            var fileInfoList = _fileLogsService.GetAllAsync(env).OrderByDescending(x => x.LastWriteTime);

            int pageNumber = page ?? 1;

            model.Logs = fileInfoList.ToPagedList(pageNumber, pageSize).ToMappedPagedList<FileInfo, FileLogVM>(); // _mapper.Map<List<FileLogVM>>(fileInfoList);
            model.Env = env;
            model.Description = $"Requests from {model.Env}";

            return model;
        }

        [Route("[controller]/Details/{env}/{fileName}")]
        public IActionResult Details(string fileName, EnvType? env)
        {
            _logger.LogDebug($"[{nameof(Details)}] Start for name = '{fileName}' and env = '{env}'");

            var fileInfo = _fileLogsService.GetFileByNameAsync(env.Value, fileName);

            FileLogVM requestVM = _mapper.Map<FileLogVM>(fileInfo);
            requestVM.Env = env;

            using (var sr = fileInfo.OpenText())
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    requestVM.FileContent += $"{s}\n\n";
                }
            }

            return View(requestVM);
        }
    }
}
