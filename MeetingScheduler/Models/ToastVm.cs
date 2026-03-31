using MeetingScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.UI.Models
{
    public class ToastVm
    {
        public ToastAlertType Type { get; set; }
        public string Message { get; set; }
    }
}
