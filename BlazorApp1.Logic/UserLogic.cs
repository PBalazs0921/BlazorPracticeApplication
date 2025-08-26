using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using BlazorApp1.Logic.Dto;

namespace BlazorApp1.Logic;

public class UserLogic(Repository<User> repository, DtoProvider dtoProvider)
{
    private readonly Mapper _mapper = dtoProvider.Mapper;


    public async Task<bool> CreateUserAsync(UserCreateDto dto)
    {
        var user = _mapper.Map<User>(dto);
        await repository.CreateAsync(user);
        return true;
    }


    public async Task<IEnumerable<UserViewDto>> GetAllUsersAsync()
    {
        Console.WriteLine("GetAllUsers called");
        var users = await repository.GetAllAsync();
        return users.Select(x => _mapper.Map<UserViewDto>(x));
    }

    public async Task<bool> UpdateUserAsync(UserUpdateDto dto)
    {
        var userToUpdate = await repository.FindByIdAsync(dto.Id);
        if (userToUpdate != null)
        {
            _mapper.Map(dto, userToUpdate);
            await repository.UpdateAsync(userToUpdate);
            return true;
        }

        return false;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        Console.WriteLine("delete user pressed " + id);
    
        var user = await repository.FindByIdAsync(id);
        if (user == null) return false;

        await repository.DeleteAsync(user);
        return true;
    }
}