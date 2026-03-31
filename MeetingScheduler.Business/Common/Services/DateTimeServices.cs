

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingScheduler.Domain.Common.Interfaces;

namespace MeetingScheduler.Infrastructure.Common.Services
{
    public class DateTimeServices : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}
