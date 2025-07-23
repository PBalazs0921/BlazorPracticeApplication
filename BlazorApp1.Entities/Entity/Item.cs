using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlazorApp1.Entities.Helper;

namespace BlazorApp1.Entities.Entity;

public class Item:IIdEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public int CategoryId { get; set; }
    public required Category Category { get; set; }
    [Required]
    public int Price { get; set; }
}