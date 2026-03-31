using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MeetingScheduler.Domain.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {

        private readonly IApplicationDbContext _context;
        private readonly ILogger<LanguageRepository> _logger;
        private readonly IDateTimeService _dateTimeService;

        public LanguageRepository(ILogger<LanguageRepository> logger, IApplicationDbContext context, IDateTimeService dateTimeService)
        {
            _context = context;
            _logger = logger;
            _dateTimeService = dateTimeService;
        }

        public async Task<List<Language>> GetAll()
        {
            return await _context.Languages.Where(x => x.IsActive == true)
                        .ToListAsync();
        }
    }

}
