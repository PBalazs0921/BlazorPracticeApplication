using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities;
using BlazorApp1.Entities.Dto;

namespace BlazorApp1.Logic;

public class UserLogic
{
    private readonly Repository<User> _repository;
    public Mapper mapper;
    public UserLogic(Repository<User> repository, DtoProvider dtoProvider)
    {
        _repository = repository;
        mapper = dtoProvider.mapper;
    }


    public bool CreateUser(UserCreateDto dto)
    {
        var User = mapper.Map<User>(dto);
        _repository.Create(User);
        return true;
    }

    public IEnumerable<UserViewDto> GetAllUsers()
    {
        Console.WriteLine("GetAllUsers called");
        return _repository.GetAll().Select(x=> mapper.Map<UserViewDto>(x));
    }
    
    public bool UpdateUser(UserUpdateDto dto)
    {
        var UserToUpdate = _repository.FindById(dto.Id);
        if (UserToUpdate != null && UserToUpdate.Id == dto.Id)
        {
            mapper.Map(dto, UserToUpdate);
            _repository.Update(UserToUpdate);
            return true;
        }
        return false;
    }
    public bool DeleteUser(int id)
    {
        Console.WriteLine("delete user pressed " + id);
        var user = _repository.FindById(id);
        if (user == null) return false;

        _repository.Delete(user);
        return true;
    }
}