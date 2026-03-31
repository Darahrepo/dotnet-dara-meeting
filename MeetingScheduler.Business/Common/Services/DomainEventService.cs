using MeetingScheduler.Domain.Common;
using MeetingScheduler.Domain.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Common.Services
{
    public class DomainEventService :IDomainEventService
    {

        private readonly ILogger<DomainEventService> _logger;

        public DomainEventService(ILogger<DomainEventService> logger)
        {
            _logger = logger;
        }

        //public async Task Publish(DomainEvent domainEvent)
        //{
        //    _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        //    await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
        //}

        //private INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
        //{
        //    return (INotification)Activator.CreateInstance(
        //        typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);
        //}
        
    }
}
