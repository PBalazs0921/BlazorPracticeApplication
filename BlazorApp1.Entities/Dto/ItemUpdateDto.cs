using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Entities.Dto;

public class ItemUpdateDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; } 

    public int CategoryId { get; set; }

    public int Price { get; set; }
}