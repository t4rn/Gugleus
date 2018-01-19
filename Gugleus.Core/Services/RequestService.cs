﻿using AutoMapper;
using Gugleus.Core.Domain;
using Gugleus.Core.Domain.Requests;
using Gugleus.Core.Dto.Input;
using Gugleus.Core.Dto.Output;
using Gugleus.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gugleus.Core.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IMapper _mapper;
        private readonly IUtilsService _utilsService;
        private readonly ILogger<RequestService> _logger;
        private readonly IConfiguration _configuration;

        public RequestService(IRequestRepository requestRepository, IMapper mapper,
            IUtilsService utilsService, ILogger<RequestService> logger, IConfiguration configuration)
        {
            _requestRepository = requestRepository;
            _mapper = mapper;
            _utilsService = utilsService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IdResultDto<long>> AddRequestAsync<T>(T requestDto, WsClient wsClient)
            where T : AbstractRequestDto
        {
            IdResultDto<long> result = new IdResultDto<long>();

            try
            {
                // saving img to disk
                if (requestDto is PostDto)
                {
                    PreProcessingRequest((requestDto as PostDto).Image);
                }

                // preparing request
                Request request = PrepareRequest(requestDto, wsClient);

                // saving to db
                long id = await _requestRepository.AddRequestAsync(request);

                // preparing result
                if (id > 0)
                {
                    result.IsOk = true;
                    result.Id = id;
                    result.Message = "Request successfully added to queue.";
                }
                else
                {
                    result.Message = "Something went wrong while adding request to db...";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(AddRequestAsync)}] Ex: {ex}");
                result.Message = $"Exception occured: {ex.Message}";
            }

            return result;
        }

        public async Task<RequestResponseDto<T>> GetRequestResponseAsync<T>(long id, DictionaryItem.RequestType requestType) where T : class
        {
            RequestResponseDto<T> result = new RequestResponseDto<T>();

            try
            {
                Request request = await _requestRepository.GetRequestWithQueueAsync(id, requestType.ToString());

                if (request != null)
                {
                    result = PrepareRequestResponse<T>(request);
                }
                else
                {
                    result.Error = $"Request with Id: {id} not found...";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{nameof(GetRequestResponseAsync)}] Ex: {ex}");
                result.Error = $"Exception occured: {ex.Message}";
            }

            return result;
        }

        public async Task<RequestSummaryDto<DateFilterDto>> GetStatsByDate(DateTime from, DateTime to)
        {
            RequestSummaryDto<DateFilterDto> result = new RequestSummaryDto<DateFilterDto>();

            result.Filter = new DateFilterDto { From = from.ToShortDateString(), To = to.ToShortDateString() };

            List<RequestStat> requestStats = await _requestRepository.GetStatsByDate(from, to);
            //result.Jobs = _mapper.Map<List<RequestTypeStatDto>>(requestStats);
            var groupedByType = requestStats.GroupBy(rs => rs.Type);
            foreach (var gr in groupedByType)
            {
                RequestTypeStatDto jobStat = new RequestTypeStatDto();
                jobStat.Type = gr.Key;
                jobStat.Summary = _mapper.Map<List<SummaryDto>>(gr);
                long avarageTicks = Convert.ToInt64(gr.Average(x => x.Avg?.Ticks));
                jobStat.AvgProcessTime = new TimeSpan(avarageTicks);

                result.Jobs.Add(jobStat);
            }

            //var groupedByType = result.Jobs.GroupBy(j => j.Type).Select(g => new { g.Key, Count = g.Sum(d => d.Amount) });
            //foreach (var item in groupedByType)
            //    result.Summary.Add(new SummaryDto { Status = item.Key, Count = item.Count });

            return result;
        }

        private Request PrepareRequest(AbstractRequestDto requestDto, WsClient wsClient)
        {
            Request request = new Request();
            request.Type = new DictionaryItem(requestDto.RequestType);
            request.Input = _utilsService.SerializeToJson(requestDto);
            request.WsClient = wsClient;

            return request;
        }

        private RequestResponseDto<T> PrepareRequestResponse<T>(Request request)
        {
            RequestResponseDto<T> dto = new RequestResponseDto<T>();

            dto.Id = request.Id;
            dto.Status = request.Queue?.Status?.Code;
            dto.Error = request.Queue?.ErrorMsg;

            if (dto.Status == DictionaryItem.RequestStatus.DONE.ToString())
            {
                if (!string.IsNullOrWhiteSpace(request.Output))
                {
                    dto.Info = _utilsService.DeserializeFromJson<T>(request.Output);
                }
                else
                {
                    string errMsg = "Empty json from db";
                    dto.Error = !string.IsNullOrWhiteSpace(dto.Error) ? $"{dto.Error} | {errMsg}" : errMsg;
                }
            }

            return dto;
        }

        private void PreProcessingRequest(ImageDto imageDto)
        {
            if (!string.IsNullOrWhiteSpace(imageDto?.Content))
            {
                int i = 0;
                string fileFolder = _configuration["img-dir"];
                string fileName = $"{fileFolder}{Guid.NewGuid().ToString()}.{imageDto.Format}";

                bool fileExists = File.Exists(fileName);

                // checking if file exists
                while (fileExists && i < 10)
                {
                    fileName = $"{fileFolder}{Guid.NewGuid().ToString()}.{imageDto.Format}";
                    fileExists = File.Exists(fileName);
                    i++;
                }

                if (!fileExists)
                {
                    if (!Directory.Exists(fileFolder)) Directory.CreateDirectory(fileFolder);

                    // saving file
                    File.WriteAllBytes(fileName, Convert.FromBase64String(imageDto.Content));
                    // changing content of image to file location
                    imageDto.Content = fileName;
                }
                else
                {
                    _logger.LogError($"[{nameof(PreProcessingRequest)}] fileExists: '{fileName}' - tried '{i}' times...");
                }
            }
        }
    }
}
