using BlazorApp1.Entities.Entity;

namespace BlazorApp1.Entities.Dto;

public class ItemViewDto
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public int Price { get; set; }
}