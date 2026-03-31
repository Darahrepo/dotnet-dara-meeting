using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace MeetingScheduler.Domain.Common.Models
{
    public class MeetingValidity
    {
        private DateTime date;
        private TimeSpan from;
        private TimeSpan to;
        private int hostId;
        private int roomId;
        private int meetingId;
        private int webinarId;
        private string locationType;
        private bool isCeo;
        private bool isWebex;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public TimeSpan From
        {
            get { return from; }
            set { from = value; }
        }
        public TimeSpan To
        {
            get { return to; }
            set { to = value; }
        }
        public int HostId
        {
            get { return hostId; }
            set { hostId = value; }
        }
        public int RoomId
        {
            get { return roomId; }
            set { roomId = value; }
        }
        public int MeetingId
        {
            get { return meetingId; }
            set { meetingId = value; }
        }
        public int WebinarId
        {
            get { return webinarId; }
            set { webinarId = value; }
        }
        public char LocationType
        {
            get { return Convert.ToChar(locationType); }
            set { locationType = value + ""; }
        }
        public bool IsCeo
        {
            get { return isCeo; }
            set { isCeo = value; }
        }
        public bool IsWebex
        {
            get { return isWebex; }
            set { isWebex = value; }
        }

    }
}
