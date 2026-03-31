using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Common.Interfaces
{
    public interface IMeetingValidationService
    {
        Task<MeetingAvailability> CheckIfTimeAvailableForHost(MeetingValidity meetingValidity, ZoomUserType zoomAccount = ZoomUserType.Main);    }
}
