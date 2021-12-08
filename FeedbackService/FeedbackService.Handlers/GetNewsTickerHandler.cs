using FeedbackService.Core.Domains;
using FeedbackService.Core.Interfaces.Repositories;
using HelpMyStreet.Contracts;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.Handlers
{
    public class GetNewsTickerHandler : IRequestHandler<NewsTickerRequest, NewsTickerResponse>
    {
        private readonly IRepository _repository;

        public GetNewsTickerHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<NewsTickerResponse> Handle(NewsTickerRequest request, CancellationToken cancellationToken)
        {
            NewsTickerResponse response = new NewsTickerResponse()
            {
                Messages = new List<NewsTickerMessage>()
            };

            var feedbackSummary = await _repository.FeedbackSummary(request.GroupId);

            var volunteerMessages = GetNewsTickerMessage(feedbackSummary.Where(x => x.RequestRoles == RequestRoles.Volunteer), " from volunteers");

            if(volunteerMessages!=null)
            {
                response.Messages.Add(volunteerMessages);
            }

            var requestorMessages = GetNewsTickerMessage(feedbackSummary.Where(x => x.RequestRoles == RequestRoles.Requestor || x.RequestRoles == RequestRoles.Recipient), " from people requesting or receiving help");

            if (requestorMessages != null)
            {
                response.Messages.Add(requestorMessages);
            }

            var allMessages = GetNewsTickerMessage(feedbackSummary, string.Empty);

            if (allMessages != null)
            {
                response.Messages.Add(allMessages);
            }

            return response;
        }

        private NewsTickerMessage GetNewsTickerMessage(IEnumerable<FeedbackRatingCount> feedback, string message)
        {
            NewsTickerMessage newsTickerMessage = null;
            var totalFeedback = feedback.Sum(x => x.Value);

            if (feedback != null && totalFeedback > 10)
            {
                var positivefeedback = feedback.Where(x => x.FeedbackRating == FeedbackRating.HappyFace).Sum(x => x.Value);
                var positivefeedbackPercentage = (positivefeedback / totalFeedback) * 100;
                if (positivefeedbackPercentage > 90)
                {
                    newsTickerMessage = new NewsTickerMessage()
                    {
                        Value = positivefeedbackPercentage,
                        Message = $"**{ Math.Round(positivefeedbackPercentage,1) }%** positive feedback{message}"
                    };
                }                
            }
            
            return newsTickerMessage;
        }
    }    
}
