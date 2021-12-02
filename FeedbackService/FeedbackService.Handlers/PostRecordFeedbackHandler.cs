using FeedbackService.Core.Exceptions;
using FeedbackService.Core.Interfaces.Repositories;
using FeedbackService.Core.Interfaces.Services;
using HelpMyStreet.Contracts.FeedbackService.Request;
using HelpMyStreet.Contracts.FeedbackService.Response;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Handlers
{
    public class PostRecordFeedbackHandler : IRequestHandler<PostRecordFeedbackRequest, PostRecordFeedbackResponse>
    {
        private readonly IRepository _repository;
        private readonly IRequestService _requestService;

        public PostRecordFeedbackHandler(IRepository repository, IRequestService requestService)
        {
            _repository = repository;
            _requestService = requestService;
        }

        public async Task<PostRecordFeedbackResponse> Handle(PostRecordFeedbackRequest request, CancellationToken cancellationToken)
        {
            try
            {
                bool feedbackExists = await _repository.FeedbackExists(request.JobId, request.RequestRoleType.RequestRole, request.UserId);

                if (feedbackExists)
                {
                    throw new FeedbackExistsException();
                }
                else
                {
                    var jobDetails = await _requestService.GetJobDetails(request.JobId);

                    if (jobDetails != null)
                    {
                        int referringGroupId = jobDetails.JobSummary.ReferringGroupID;
                        bool success = await _repository.AddFeedback(request, referringGroupId);

                        var response = new PostRecordFeedbackResponse()
                        {
                            Success = success
                        };
                        return response;
                    }
                    else
                    {
                        throw new Exception($"Unable to find job details for job id {request.JobId }");
                    }
                }
            }
            catch(FeedbackExistsException exc)
            {
                throw exc;
            }
        }
    }
}
