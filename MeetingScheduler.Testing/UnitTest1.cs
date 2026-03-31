using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Common.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace MeetingScheduler.Testing
{
    [TestClass]
    public class Test
    {
        private readonly IZoomService _zoomService;

        public Test(IZoomService zoomService)
        {
            _zoomService = zoomService;
        }

        [TestMethod]
        public async Task<int> TestAdd()
        {
            ZoomMeetingDetails zoomMeetingDetails = new ZoomMeetingDetails();
            zoomMeetingDetails.StartDateTime = DateTime.Now;
            zoomMeetingDetails.DurationInMinutes = 60;
            var result = await _zoomService.CheckMeetingExists(zoomMeetingDetails);
            return 1;
        }
    }
}