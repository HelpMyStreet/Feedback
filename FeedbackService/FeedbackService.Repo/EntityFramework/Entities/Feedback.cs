using System;
using System.Collections.Generic;
using System.Text;

namespace FeedbackService.Repo.EntityFramework.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public DateTime FeedbackDate { get; set; }
        public int JobId { get; set; }
        public int? ReferringGroupId { get; set; }
        public int? UserId { get; set; }
        public byte RequestRoleTypeId { get; set; }
        public byte FeedbackRatingTypeId { get; set; }
    }
}
