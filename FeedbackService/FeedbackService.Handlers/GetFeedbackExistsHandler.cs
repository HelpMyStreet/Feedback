using FeedbackService.Core.Interfaces.Repositories;
using HelpMyStreet.Contracts.FeedbackService.Request;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Handlers
{
    public class GetFeedbackExistsHandler : IRequestHandler<GetFeedbackExistsRequest, bool>
    {
        private readonly IRepository _repository;

        public GetFeedbackExistsHandler(IRepository repository)
        {
            _repository = repository;
        }

        public Task<bool> Handle(GetFeedbackExistsRequest request, CancellationToken cancellationToken)
        {
            bool success = _repository.FeedbackExists(request.JobId, request.RequestRoleType.RequestRole, request.UserId).Result;
            return Task.FromResult(success);
        }
    }
}
