using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Hangfire
{
    public interface IBackgroundJob
    {
        string AddEnque(Expression<Action> methodCall);

        string AddEnque<T>(Expression<Action<T>> methodCall);

        string AddContinuations(Expression<Action> methodCall, string jobid);

        string AddContinuations<T>(Expression<Action<T>> methodCall, string jobid);

        string AddSchedule(Expression<Action> methodCall, RecuringTime recuringTime, double time);

        string AddSchedule<T>(Expression<Action<T>> methodCall, RecuringTime recuringTime, double time);
    }

    public enum RecuringType
    {
        Daily,
        Minutely,
        Hourly,
        Weekly,
        Monthly,
        Yearly
    }

    public enum RecuringTime
    {
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
        Day
    }
}
