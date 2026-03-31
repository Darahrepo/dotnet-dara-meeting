using MeetingScheduler.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeetingScheduler.Domain.Entities;
using AutoMapper;
using MeetingScheduler.Infrastructure.Interfaces;

namespace MeetingScheduler.Infrastructure.Services.SampleUsers
{
    public class SampleUserService : ISampleUserService
    {
        private readonly ISampleUserRepository _sampleUserRepository;
        private readonly IMapper _mapper;

        public SampleUserService(ISampleUserRepository sampleUserRepository, IMapper mapper)
        {
            _sampleUserRepository = sampleUserRepository;
            _mapper = mapper;
        }

        public async Task<List<SampleUserDto>> GetAll()
        {
            var entity = await _sampleUserRepository.GetAll();
            List<SampleUserDto> users = _mapper.Map<List<SampleUserDto>>(entity);

            return users;
        }

        public async Task<SampleUserDto> GetById(int id)
        {
            var entity = await _sampleUserRepository.GetById(id);
            SampleUserDto user = _mapper.Map<SampleUserDto>(entity);

            return user;
        }

        public async Task<int> Create(SampleUserDto sampleUser,CancellationToken cancellationToken)
        {
            SampleUser entity = _mapper.Map<SampleUser>(sampleUser);

            return await _sampleUserRepository.Create(entity, cancellationToken);
        }

        public async Task<int> Update(SampleUserDto sampleUser, CancellationToken cancellationToken)
        {
            SampleUser entity = _mapper.Map<SampleUser>(sampleUser);

            return await _sampleUserRepository.Update(entity, cancellationToken);
        }

        public async Task<int> Delete(int id, CancellationToken cancellationToken)
        {

            return await _sampleUserRepository.Delete(id, cancellationToken);
        }
    }
}
