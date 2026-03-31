using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Enums
{
    public enum ApprovalStatus : int
    {
        Pending = 'P',
        Approved = 'A',
        Rejected = 'R',
        Cancelled = 'C'
    }
}
