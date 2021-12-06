using FeedbackService.Core.Domains;
using HelpMyStreet.Contracts.FeedbackService.Request;
using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeedbackService.Core.Interfaces.Repositories
{
    public interface IRepository
    {
        Task<IEnumerable<FeedbackRatingCount>> FeedbackSummary(int? groupId);
        Task<bool> AddFeedback(PostRecordFeedbackRequest request, int referringGroupId);
        Task<bool> FeedbackExists(int jobId, RequestRoles requestRoles, int? userId);

    }
}
