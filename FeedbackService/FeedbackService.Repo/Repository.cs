using AutoMapper;
using FeedbackService.Core.Domains;
using FeedbackService.Core.Interfaces.Repositories;
using HelpMyStreet.Contracts.FeedbackService.Request;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using System;
using System.Collections.Generic;
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

        public async Task<bool> AddFeedback(PostRecordFeedbackRequest request, int referringGroupId)
        {
            _context.Feedback.Add(new EntityFramework.Entities.Feedback()
            {
                JobId = request.JobId,
                UserId = request.UserId,
                ReferringGroupId = referringGroupId,
                RequestRoleTypeId = (byte)request.RequestRoleType.RequestRole,
                FeedbackRatingTypeId = (byte)request.FeedbackRatingType.FeedbackRating
            }) ;

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

        public async Task<IEnumerable<FeedbackRatingCount>> FeedbackSummary(int? groupId)
        {            byte happyFace = (byte)FeedbackRating.HappyFace;
            byte sadFace = (byte)FeedbackRating.SadFace;


            return _context.Feedback
                .Where(x => (x.FeedbackRatingTypeId == happyFace || x.FeedbackRatingTypeId == sadFace)
                && x.ReferringGroupId == (groupId.HasValue ? groupId.Value : x.ReferringGroupId))
                .GroupBy(x=> new { x.FeedbackRatingTypeId, x.RequestRoleTypeId })
                .Select(g => new FeedbackRatingCount { FeedbackRating = (FeedbackRating) g.Key.FeedbackRatingTypeId, RequestRoles= (RequestRoles) g.Key.RequestRoleTypeId, Value = (double) g.Count()});
        }
    }
}
