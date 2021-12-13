using FeedbackService.Core.Domains;
using FeedbackService.Core.Interfaces.Repositories;
using FeedbackService.Handlers;
using HelpMyStreet.Contracts;
using HelpMyStreet.Contracts.FeedbackService.Request;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.EqualityComparers;
using HelpMyStreet.Utils.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FeedbackService.UnitTests.Handlers
{
    public class GetNewsTickerHandlerTests
    {
        private Mock<IRepository> _repository;
        private GetNewsTickerHandler _classUnderTest;
        private List<FeedbackRatingCount> _feedbackRatingCount;

        [SetUp]
        public void Setup()
        {
            SetupRepository();
            _classUnderTest = new GetNewsTickerHandler(_repository.Object);
        }

        private void SetupRepository()
        {
            _repository = new Mock<IRepository>();
            _repository.Setup(x => x.FeedbackSummary(It.IsAny<int?>()))
                .ReturnsAsync(() => _feedbackRatingCount);
        }

        [TestCase(100, 10, 0, 0, 0, 0, "**90.9%** positive feedback from volunteers", "", 2)]
        [TestCase(0, 0, 8, 2, 0, 0,  "", "", 0)]
        [TestCase(0, 0, 10, 5, 0, 0, "", "", 0)]
        [TestCase(0, 0 , 0, 0 , 100, 10, "", "**90.9%** positive feedback from people requesting or receiving help", 2)]
        [TestCase(0, 0, 0, 0, 8, 2, "", "", 0)]
        [TestCase(0, 0, 0, 0, 10, 5, "", "", 0)]
        [TestCase(100, 10, 0, 0, 100, 10, "**90.9%** positive feedback from volunteers", "**90.9%** positive feedback from people requesting or receiving help", 3)]
        [TestCase(100, 10, 90, 1, 100, 10, "**90.9%** positive feedback from volunteers", "**94.5%** positive feedback from people requesting or receiving help", 3)]

        [Test]
        public async Task HappyPath(
            int volunteerHappyCount, int volunteerSadCount, 
            int requestorHappyCount, int requestorSadCount,
            int recipientHappyCount, int recipientSadCount,
            string volunteerMessage, string requestorMessage,
            int messageCount)
        {
            int? groupId = -3;
            _feedbackRatingCount = new List<FeedbackRatingCount>();
            _feedbackRatingCount.Add(new FeedbackRatingCount() { RequestRoles = RequestRoles.Volunteer, Value = volunteerHappyCount, FeedbackRating = FeedbackRating.HappyFace });
            _feedbackRatingCount.Add(new FeedbackRatingCount() { RequestRoles = RequestRoles.Volunteer, Value = volunteerSadCount, FeedbackRating = FeedbackRating.SadFace });
            _feedbackRatingCount.Add(new FeedbackRatingCount() { RequestRoles = RequestRoles.Requestor, Value = requestorHappyCount, FeedbackRating = FeedbackRating.HappyFace });
            _feedbackRatingCount.Add(new FeedbackRatingCount() { RequestRoles = RequestRoles.Requestor, Value = requestorSadCount, FeedbackRating = FeedbackRating.SadFace });
            _feedbackRatingCount.Add(new FeedbackRatingCount() { RequestRoles = RequestRoles.Recipient, Value = recipientHappyCount, FeedbackRating = FeedbackRating.HappyFace });
            _feedbackRatingCount.Add(new FeedbackRatingCount() { RequestRoles = RequestRoles.Recipient, Value = recipientSadCount, FeedbackRating = FeedbackRating.SadFace });


            NewsTickerResponse response = await _classUnderTest.Handle(new NewsTickerRequest()
            {
                GroupId = groupId
            }, CancellationToken.None);

            if (!string.IsNullOrEmpty(volunteerMessage))
            {
                Assert.AreEqual(1, response.Messages.Count(x => x.Message == volunteerMessage));
            }

            if (!string.IsNullOrEmpty(requestorMessage))
            {
                Assert.AreEqual(1, response.Messages.Count(x => x.Message == requestorMessage));
            }

            Assert.AreEqual(messageCount, response.Messages.Count);
        }
    }
}
