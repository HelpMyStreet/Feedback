using HelpMyStreet.Utils.Enums;

namespace FeedbackService.Core.Domains
{
    public class FeedbackRatingCount
    {
        public RequestRoles RequestRoles { get; set; }
        public FeedbackRating FeedbackRating { get; set; }
        public double Value { get; set; }
    }
}
