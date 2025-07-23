using AutoMapper;
using BlazorApp1.Entities;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;

namespace BlazorApp1.Logic.Dto;

public class DtoProvider
{
    public Mapper mapper {
        get;
    }
    
    public DtoProvider()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Booking, BookingViewDto>();
            cfg.CreateMap<BookingCreateDto, Booking>();
            cfg.CreateMap<BookingUpdateDto, Booking>();

            cfg.CreateMap<Item, ItemViewDto>();
            cfg.CreateMap<ItemCreateDto, Item>();
            cfg.CreateMap<ItemUpdateDto, Item>();
            
            cfg.CreateMap<Category, CategoryViewDto>();
            cfg.CreateMap<CategoryCreateDto, Category>();
            cfg.CreateMap<CategoryUpdateDto, Category>();

            cfg.CreateMap<User, UserViewDto>();
            cfg.CreateMap<UserCreateDto, User>();
            cfg.CreateMap<UserUpdateDto, User>();


        });
        mapper = new Mapper(config);
    }
}