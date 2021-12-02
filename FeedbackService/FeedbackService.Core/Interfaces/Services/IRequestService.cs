using HelpMyStreet.Contracts.RequestService.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackService.Core.Interfaces.Services
{
    public interface IRequestService
    {
        Task<GetJobDetailsResponse> GetJobDetails(int jobId);
    }
}
