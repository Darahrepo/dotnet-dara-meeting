using MeetingScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Domain.Interfaces
{
    public interface ILanguageRepository
    {
        Task<List<Language>> GetAll();
    }
}
