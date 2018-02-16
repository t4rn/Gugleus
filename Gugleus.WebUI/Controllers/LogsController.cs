using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.WebUI.Models.Logs;
using Gugleus.WebUI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Gugleus.WebUI.Controllers
{
    public class LogsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IFileLogsService _fileLogsService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(IMapper mapper, IFileLogsService fileLogsService, ILogger<LogsController> logger)
        {
            _mapper = mapper;
            _fileLogsService = fileLogsService;
            _logger = logger;
        }

        public IActionResult Dev()
        {
            // TODO: pagination
            FileLogListVM model = PrepareModel(EnvType.Dev);
            return View("FileLogsList", model);
        }

        public IActionResult Rc()
        {
            FileLogListVM model = PrepareModel(EnvType.Rc);
            return View("FileLogsList", model);
        }

        public IActionResult Prod()
        {
            FileLogListVM model = PrepareModel(EnvType.Prod);
            return View("FileLogsList", model);
        }

        private FileLogListVM PrepareModel(EnvType env)
        {
            FileLogListVM model = new FileLogListVM();
            var fileInfoList = _fileLogsService.GetAllAsync(env).OrderByDescending(x => x.LastWriteTime);

            model.Logs = _mapper.Map<List<FileLogVM>>(fileInfoList);
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
