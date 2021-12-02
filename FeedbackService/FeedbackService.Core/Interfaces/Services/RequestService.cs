using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Core.Interfaces.Services
{
    public class RequestService : IRequestService
    {
        private readonly IHttpClientWrapper _httpClientWrapper;

        public RequestService(IHttpClientWrapper httpClientWrapper)
        {
            _httpClientWrapper = httpClientWrapper;
        }

        public async Task<GetJobSummaryResponse> GetJobSummary(int jobId)
        {
            string path = $"/api/GetJobSummary?jobID={jobId}&userId=-1";
            string absolutePath = $"{path}";
            using (HttpResponseMessage response = await _httpClientWrapper.GetAsync(HttpClientConfigName.RequestService, absolutePath, CancellationToken.None).ConfigureAwait(false))
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var getJobsResponse = JsonConvert.DeserializeObject<ResponseWrapper<GetJobSummaryResponse, RequestServiceErrorCode>>(jsonResponse);
                if (getJobsResponse.HasContent && getJobsResponse.IsSuccessful)
                {
                    return getJobsResponse.Content;
                }
                return null;
            }
        }
    }
}
