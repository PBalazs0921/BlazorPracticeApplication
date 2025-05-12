using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities;
using BlazorApp1.Entities.Dto;

namespace BlazorApp1.Logic;

public class CategoryLogic
{
    private readonly Repository<Category> _repository;
    public Mapper mapper;

    public CategoryLogic(Repository<Category> repository,DtoProvider dtoProvider)
    {
        _repository = repository;
        mapper = dtoProvider.mapper;
    }

    public bool CreateItem(CategoryCreateDto dto)
    {
        var category = mapper.Map<Category>(dto);
        _repository.Create(category);
        return true;
    }

    public IEnumerable<CategoryViewDto> GetAllItems()
    {
        Console.WriteLine("GetAllItems called");
        return _repository.GetAll().Select(x=> mapper.Map<CategoryViewDto>(x));
    }
    
    public bool UpdateItem(CategoryUpdateDto dto)
    {
        var CategoryToUpdate = _repository.FindById(dto.Id);
        if (CategoryToUpdate != null && CategoryToUpdate.Id == dto.Id)
        {
            mapper.Map(dto, CategoryToUpdate);
            _repository.Update(CategoryToUpdate);
            return true;
        }

        return false;
    }

    public bool DeleteCategory(int id)
    {
        Console.WriteLine("DeleteItem pressed: " + id);
        var item = _repository.FindById(id);
        if (item == null) return false;

        _repository.Delete(item);
        return true;
    }
}