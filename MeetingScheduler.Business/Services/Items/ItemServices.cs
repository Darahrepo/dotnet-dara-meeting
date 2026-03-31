using AutoMapper;
using MeetingScheduler.Domain.Entities;
using MeetingScheduler.Domain.Interfaces;
using MeetingScheduler.Infrastructure.Services.MeetingItems;
using MeetingScheduler.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Services.Items
{
    public class ItemServices : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public ItemServices(IItemRepository ItemRepository, IMapper mapper)
        {
            _itemRepository = ItemRepository;
            _mapper = mapper;
        }

        public async Task<List<ItemDto>> GetAll()
        {
            var entity = await _itemRepository.GetAll();
            List<ItemDto> Items = _mapper.Map<List<ItemDto>>(entity);

            return Items;
        }

        public async Task<ItemDto> GetById(int id)
        {
            var entity = await _itemRepository.GetById(id);
            ItemDto Item = _mapper.Map<ItemDto>(entity);

            return Item;
        }

        public async Task<int> Create(ItemDto Item, CancellationToken cancellationToken)
        {
            Item entity = _mapper.Map<Item>(Item);

            return await _itemRepository.Create(entity, cancellationToken);
        }

        public async Task<int> Update(ItemDto Item, CancellationToken cancellationToken)
        {
            Item entity = _mapper.Map<Item>(Item);

            return await _itemRepository.Update(entity, cancellationToken);
        }

        public async Task<int> Delete(int id, CancellationToken cancellationToken)
        {

            return await _itemRepository.Delete(id, cancellationToken);
        }
    }
}
