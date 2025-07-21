using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities;
using BlazorApp1.Entities.Dto;

namespace BlazorApp1.Logic;

public class ItemLogic
{
    private readonly Repository<Item> _repository;
    public Mapper mapper;

    public ItemLogic(Repository<Item> repository,DtoProvider dtoProvider)
    {
        _repository = repository;
        mapper = dtoProvider.mapper;
    }

    public bool CreateItem(ItemCreateDto dto)
    {
        var Item = mapper.Map<Item>(dto);
        _repository.Create(Item);
        return true;
    }

    public IEnumerable<ItemViewDto> GetAllItems()
    {
        Console.WriteLine("GetAllItems called");
        return _repository.GetAll().Select(x=> mapper.Map<ItemViewDto>(x));
    }

    public bool UpdateItem(ItemUpdateDto dto)
    {
        var ItemToUpdate = _repository.FindById(dto.Id);
        if (ItemToUpdate != null && ItemToUpdate.Id == dto.Id)
        {
            mapper.Map(dto, ItemToUpdate);
            _repository.Update(ItemToUpdate);
            return true;
        }
        return false;
    }

    public bool DeleteItem(int id)
    {
        Console.WriteLine("DeleteItem pressed: " + id);
        var item = _repository.FindById(id);
        if (item == null) return false;

        _repository.Delete(item);
        return true;
    }
}