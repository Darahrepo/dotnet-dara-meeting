using AutoMapper;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Services.Languages
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IMeetingAttendeesRepository _meetingAttendeesRepository;
        private readonly IMapper _mapper;
        private readonly int currentUser;
        private readonly IDateTimeService _dateTime;

        public LanguageService(ILanguageRepository languageRepository, IDateTimeService dateTime, IMapper mapper)
        {
            _languageRepository = languageRepository;
            _mapper = mapper;
            _dateTime = dateTime;
        }

        public async Task<List<Language>> GetAll()
        {
            List<Language> languages = new List<Language>();
            try
            {
                languages = await _languageRepository.GetAll();
            }
            catch (Exception Ex)
            {
                throw;
            }

            return languages;
        }
    }
}
