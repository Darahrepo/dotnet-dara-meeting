using MeetingScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Interfaces
{
    public interface ILanguageService
    {
        Task<List<Language>> GetAll();
    }
}
