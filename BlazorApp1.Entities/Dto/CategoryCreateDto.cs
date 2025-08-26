using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Entities.Dto;

public class CategoryCreateDto
{
    [Required]
    public string? Name { get; set; }
}