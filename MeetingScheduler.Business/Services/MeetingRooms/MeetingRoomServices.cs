using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Services.MeetingRooms
{
    public class MeetingRoomServices : IMeetingRoomService
    {
        private readonly IMeetingRoomRepository _meetingRoomRepository;
        private readonly IMapper _mapper;

        public MeetingRoomServices(IMeetingRoomRepository meetingRoomRepository, IMapper mapper)
        {
            _meetingRoomRepository = meetingRoomRepository;
            _mapper = mapper;
        }

        public async Task<List<MeetingRoom>> GetAll()
        {
            var entity = new List<MeetingRoom>();
            try
            {
                entity = await _meetingRoomRepository.GetAll();
                //List<MeetingRoom> meetingRooms = _mapper.Map<List<MeetingRoom>>(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

            return entity;
        }

        public async Task<MeetingRoom> GetById(int id)
        {
            var entity = new MeetingRoom();
            try
            {
                entity = await _meetingRoomRepository.GetById(id);
            }
            catch (Exception ex )
            {
                throw ex;
            }
            return entity;
        }

        public async Task<int> Create(MeetingRoom meetingRoom, CancellationToken cancellationToken)
        {
            var result = 0;
            try
            {
                result = await _meetingRoomRepository.Create(meetingRoom, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public async Task<int> Update(MeetingRoom meetingRoom, CancellationToken cancellationToken)
        {
            var result = 0;
            try
            {
                result =  await _meetingRoomRepository.Update(meetingRoom, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;

        }

        public async Task<int> Delete(int id, CancellationToken cancellationToken)
        {
            var result = 0;
            try
            {
                result = await _meetingRoomRepository.Delete(id, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
