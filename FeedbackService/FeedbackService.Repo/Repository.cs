using AutoMapper;
using FeedbackService.Core.Interfaces.Repositories;
using HelpMyStreet.Contracts.FeedbackService.Request;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FeedbackService.Repo
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public Repository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> AddFeedback(PostRecordFeedbackRequest request)
        {
            _context.Feedback.Add(new EntityFramework.Entities.Feedback()
            {
                JobId = request.JobId,
                UserId = request.UserId,
                RequestRoleTypeId = (byte) request.RequestRoleType.RequestRole,
                FeedbackRatingTypeId = (byte) request.FeedbackRatingType.FeedbackRating
            });

            var result = await _context.SaveChangesAsync();

            if(result==1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> FeedbackExists(int jobId, RequestRoles requestRoles, int? userId)
        {
            var feedback = _context.Feedback.Where(x => x.JobId == jobId && (RequestRoles)x.RequestRoleTypeId == requestRoles && (requestRoles.LimitOneFeedbackPerRequest() || x.UserId == userId)).FirstOrDefault();
           
            if(feedback!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
