using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BlazorApp1.Entities.Helper;

namespace BlazorApp1.Entities.Entity;

public class Booking : IIdEntity
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public int Price { get; set; }
    public string? Comment { get; set; }
    [Required]
    public required AppUser User { get; set; }
}